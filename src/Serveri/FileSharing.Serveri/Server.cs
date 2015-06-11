using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileSharing.Core.Protokoli;
using FileSharing.Serveri.Modeli;
using FileSharing.Serveri.Sherbimet.Abstrakt;

namespace FileSharing.Serveri
{
    public class Server
    {
        private bool startuar;

        public Server(IRepository repository, IPathResolver pathat, IServerServiceLocator serviceLocator)
        {
            Repository = repository;
            PathResolver = pathat;
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
                //if ()
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
                try
                {
                    var tiketaId = fileKerkesa.TiketaId;
                    TransferTikete tiketa;
                    if (Transferet.TryGetValue(tiketaId, out tiketa))
                    {
                        Transferet.Remove(tiketaId);
                        if (tiketa.Kahu == KahuTransferit.Ngarkim)
                        {
                            var tempFile = PathResolver.GetTempFile();
                            var file = new FileStream(tempFile, FileMode.CreateNew);
                            // ReSharper disable once CSharpWarnings::CS4014
                            Task.Run(async () =>
                            {
                                Mesazh pergjigja;
                                try
                                {
                                    // ReSharper disable once AccessToDisposedClosure
                                    await fileKerkesa.PranoFajllAsync(file);
                                    //tiketa.Fajlli.
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
                                }
                            });
                        }
                    }
                    else
                    {
                        await fileKerkesa.KthePergjigjeAsync(new Mesazh(Header.BadTicket));
                    }
                }
                catch
                {
                    fileKerkesa.Dispose();
                }
            }
        }
    }
}
