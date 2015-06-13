using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileSharing.Core;
using FileSharing.Core.Modeli;
using FileSharing.Core.Protokoli;
using FileSharing.Serveri.Infrastruktura.Abstrakt;
using FileSharing.Serveri.Modeli;
using FileSharing.Serveri.Sherbimet.Abstrakt;

#pragma warning disable 4014

namespace FileSharing.Serveri
{
    public class Server
    {
        private readonly SemaphoreSlim transferetSemafori;
        private bool startuar;

        public Server(IRepository repository, IPathResolver pathat, IServerServiceLocator serviceLocator,
            int maxTransfere)
        {
            Repository = repository;
            PathResolver = pathat;
            this.transferetSemafori = new SemaphoreSlim(maxTransfere);
            KlientPranuesi = serviceLocator.MerrKlientPranues();
            TransferPranuesi = serviceLocator.MerrFilePranues();
            KlientetLoguar = new Dictionary<string, IKlientKomunikues>();
            Transferet = new Dictionary<int, TransferTikete>();
        }

        #region Gjendja

        // Mban sesion per shfrytezuesit e loguar.
        public Dictionary<string, IKlientKomunikues> KlientetLoguar { get; set; }

        // Ruan tiketat e transfereve ne nje dictionary.
        // Klienti ben kerkesen ne kanalin e komunikimit dhe i jepet shifra 
        // dhe me pas dorezon keto shifra ne kanalin e transferit.
        public Dictionary<int, TransferTikete> Transferet { get; private set; }

        #endregion

        #region Komponentet

        public IRepository Repository { get; set; }

        public IPathResolver PathResolver { get; set; }

        public IKlientPranues KlientPranuesi { get; private set; }

        public IFileKomunikuesPranues TransferPranuesi { get; private set; }

        #endregion

        public void Starto()
        {
            if (startuar)
            {
                return;
            }

            KlientPranuesi.Starto();
            TransferPranuesi.Starto();
            Task.Factory.StartNew(DegjoKomunikim, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(DegjoTransfer, TaskCreationOptions.LongRunning);
            startuar = true;
        }

        public void Ndalo()
        {
            startuar = false;
            KlientPranuesi.Ndalo();
            TransferPranuesi.Ndalo();
            KlientetLoguar.Clear();
            Transferet.Clear();
        }

        private async void DegjoKomunikim()
        {
            while (startuar)
            {
                var klientiKomunikuesi = await KlientPranuesi.PranoKlientAsync();
                Task.Run(() => BisedoMeKlient(klientiKomunikuesi));
            }
        }

        private async void BisedoMeKlient(IKlientKomunikues komunikuesi)
        {
            try
            {
                var kerkesa = await komunikuesi.PranoAsync();
            }
            catch
            {
                return;
            }
        }

        private async void DegjoTransfer()
        {
            while (startuar)
            {
                var fileKerkesa = await TransferPranuesi.PranoFileKomunikuesAsync();
                var tiketaId = fileKerkesa.TiketaId;
                TransferTikete tiketa;
                if (!Transferet.TryGetValue(tiketaId, out tiketa))
                {
                    await fileKerkesa.KthePergjigjeAsync(new Mesazh(Header.BadTicket));
                    continue;
                }

                Transferet.Remove(tiketaId);
                var busy = !await transferetSemafori.WaitAsync(1000);
                if (busy)
                {
                    await fileKerkesa.KthePergjigjeAsync(new Mesazh(Header.ServerBusy));
                    fileKerkesa.Dispose();
                    continue;
                }

                var fajlli = tiketa.Fajlli;
                if (tiketa.Kahu == KahuTransferit.Ngarkim)
                {
                    var tempFile = PathResolver.GetTempFile();
                    Task.Run(async () =>
                    {
                        Mesazh pergjigja;
                        try
                        {
                            var fileStream = new FileStream(tempFile, FileMode.Create);
                            try
                            {
                                await fileKerkesa.PranoFajllAsync(fileStream);
                            }
                            catch
                            {
                                fileStream.Close();
                                File.Delete(tempFile);
                                throw;
                            }


                            fajlli.Dukshmeria = Dukshmeria.Private;
                            fajlli.Madhesia = (int)fileStream.Length;
                            fajlli.DataShtimit = DateTime.Now;
                            Repository.ShtoFajll(fajlli);

                            fileStream.Close();

                            // Fajlli sukses, dergoje ne folder te vertet
                            File.Move(tempFile, PathResolver.GetFileInDataPath(fajlli.Id.ToString()));
                            pergjigja = new Mesazh(Header.Ok, XmlSerializues<FajllInfo>.Serializo(fajlli));
                        }
                        catch (HashFailException)
                        {
                            pergjigja = new Mesazh(Header.HashFail);
                        }
                        catch
                        {
                            pergjigja = new Mesazh(Header.Gabim);
                        }

                        try
                        {
                            await fileKerkesa.KthePergjigjeAsync(pergjigja);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            fileKerkesa.Dispose();
                            transferetSemafori.Release();
                        }
                    });
                }
                else // Download
                {
                    var filePath = PathResolver.GetFileInDataPath(fajlli.Id.ToString());
                    Task.Run(async () =>
                    {
                        if (!File.Exists(filePath))
                        {
                            await fileKerkesa.KthePergjigjeAsync(new Mesazh(Header.FileNotFound));
                            fileKerkesa.Dispose();
                            transferetSemafori.Release();
                            return;
                        }

                        try
                        {
                            var fileStream = new FileStream(filePath, FileMode.Open);
                            await fileKerkesa.DergoFajllAsync(Header.Ok, fileStream, (int)fileStream.Length);
                        }
                        catch
                        {
                        }
                        finally
                        {
                            fileKerkesa.Dispose();
                            transferetSemafori.Release();
                        }
                    });
                }
            }
        }
    }
}

#pragma warning restore 4014