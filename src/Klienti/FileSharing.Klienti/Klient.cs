using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using FileSharing.Core;
using FileSharing.Core.Modeli;
using FileSharing.Core.Protokoli;
using FileSharing.Klienti.Modeli;
using FileSharing.Klienti.Sherbimet.Abstrakt;

namespace FileSharing.Klienti
{
    public class Klient : IDisposable
    {
        private bool startuar = true;

        // Sinkronizimi
        private readonly SemaphoreSlim transferSemafori = new SemaphoreSlim(0);
        private readonly object transferSyncRoot = new object();

        private readonly IServerKomunikues serverKomunikuesi;

        private readonly ICoreServiceLocator coreServices;

        private readonly Queue<QueuedFileTransfer> transferetArdhshme;

        public Klient(string shfrytezuesi, string serverEmri, string serverAdresa, IServerKomunikues serverKomunikuesi, ICoreServiceLocator coreServices)
        {
            Transferet = new ObservableCollection<ProgresiTransferit>();
            transferetArdhshme = new Queue<QueuedFileTransfer>();
            Shfrytezuesi = shfrytezuesi;
            ServerEmri = serverEmri;
            ServerAdresa = serverAdresa;
            this.serverKomunikuesi = serverKomunikuesi;
            this.coreServices = coreServices;
            Task.Factory.StartNew(PritTransfere, TaskCreationOptions.LongRunning);
            //Task.Factory.StartNew(MbajAlive, TaskCreationOptions.LongRunning);
        }

        public ObservableCollection<ProgresiTransferit> Transferet { get; private set; }

        public string Shfrytezuesi { get; private set; }

        public string ServerEmri { get; private set; }

        public string ServerAdresa { get; private set; }

        public event EventHandler LidhjaHumbur;

        public event EventHandler<FajllInfoEventArgs> FajllShtuar;

        public async Task<IEnumerable<FajllInfo>> MerrFajllatAsync()
        {
            try
            {
                var pergjigja = await serverKomunikuesi.KomunikoDyAnshemAsync(new Mesazh(Header.MerrFajllat));
                return pergjigja.Header == Header.Ok ? XmlSerializuesi<FajllInfo[]>.DeserializoBajt(pergjigja.TeDhenat) : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> FshijFajllinAsync(FajllInfo fajlli)
        {
            try
            {
                var pergjigja = await serverKomunikuesi.KomunikoDyAnshemAsync(new Mesazh(Header.FileDelete, IntegerKonvertuesi.NeBajta(fajlli.Id)));
                if (pergjigja.Header == Header.Ok)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BejPublikAsync(FajllInfo fajlli)
        {
            try
            {
                var pergjigja = await serverKomunikuesi.KomunikoDyAnshemAsync(new Mesazh(Header.BejePublik, IntegerKonvertuesi.NeBajta(fajlli.Id)));
                if (pergjigja.Header == Header.Ok)
                {
                    fajlli.Dukshmeria = Dukshmeria.Publike;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> BejPrivatAsync(FajllInfo fajlli)
        {
            try
            {
                var pergjigja = await serverKomunikuesi.KomunikoDyAnshemAsync(new Mesazh(Header.BejePrivat, IntegerKonvertuesi.NeBajta(fajlli.Id)));
                if (pergjigja.Header == Header.Ok)
                {
                    fajlli.Dukshmeria = Dukshmeria.Private;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public ProgresiTransferit StartoDownloadFile(FajllInfo fajlli, string path)
        {
            var progresiTransferit = new ProgresiTransferit(fajlli.Emri, KahuTransferit.Shkarkim)
            {
                GjendjaTransferit = GjendjaTransferit.DukePritur
            };

            var transferi = new QueuedFileTransfer
            {
                Id = fajlli.Id,
                Identifier = path,
                ProgresiTransferit = progresiTransferit,
                KahuTransferit = KahuTransferit.Shkarkim
            };

            lock (transferSyncRoot)
            {
                transferetArdhshme.Enqueue(transferi);
                Transferet.Insert(0, progresiTransferit);
                transferSemafori.Release();
            }

            return progresiTransferit;
        }

        public ProgresiTransferit StartoUploadFile(string filePath)
        {
            var emri = Path.GetFileName(filePath);
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException();
            }

            var progresiTransferit = new ProgresiTransferit(emri, KahuTransferit.Ngarkim)
            {
                GjendjaTransferit = GjendjaTransferit.DukePritur
            };

            var transferi = new QueuedFileTransfer
            {
                Identifier = filePath,
                ProgresiTransferit = progresiTransferit,
                KahuTransferit = KahuTransferit.Ngarkim
            };

            lock (transferSyncRoot)
            {
                transferetArdhshme.Enqueue(transferi);
                Transferet.Insert(0, progresiTransferit);
                transferSemafori.Release();
            }

            return progresiTransferit;
        }

        private async void PritTransfere()
        {
            while (startuar)
            {
                await transferSemafori.WaitAsync();
                if (!startuar)
                {
                    return;
                }

                var transferi = transferetArdhshme.Dequeue();
                try
                {
                    if (transferi.KahuTransferit == KahuTransferit.Shkarkim)
                    {
                        // Be kerkese per download dhe prano pergjigjen nga serveri
                        var rezultati = await serverKomunikuesi.KomunikoDyAnshemAsync(new Mesazh(Header.FileDownload, IntegerKonvertuesi.NeBajta(transferi.Id)));
                        if (rezultati.Header == Header.Ok)
                        {
                            // Nese serveri kthen ok, marrim tiketen dhe e dorzojme ne transfer kanal
                            var transferStream = await serverKomunikuesi.HapTransferStreamAsync();
                            await coreServices.MerrKomunikues().ShkruajMesazhAsync(transferStream,
                                new Mesazh(Header.PaHeader, rezultati.TeDhenat));

                            var transferInfo = await serverKomunikuesi.MerrTransferInfoAsync();
                            if (transferInfo.Header == Header.Ok)
                            {
                                // Kryej transferin
                                transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.DukeTransferuar;

                                var transferuesi = coreServices.MerrTransferues();
                                transferuesi.PaketPranuar += (s, e) =>
                                {
                                    transferi.ProgresiTransferit.Perqindja = (double)e.PaketatTransferuara / e.PaketatTotal;
                                };

                                try
                                {
                                    using (var fileStream = new FileStream(transferi.Identifier, FileMode.Create))
                                    {
                                        await transferuesi.PranoStreamAsync(transferInfo.FajlliGjatesia, transferStream, fileStream);
                                        transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Perfunduar;
                                        transferi.ProgresiTransferit.StatusiText = DateTime.Now.ToString("HH:mm:ss");
                                    }
                                }
                                catch
                                {
                                    try { File.Delete(transferi.Identifier); }
                                    catch { }
                                    throw;
                                }
                            }
                            // Gabimet nga kanali transferit
                            else if (transferInfo.Header == Header.ServerBusy)
                            {
                                transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                                transferi.ProgresiTransferit.StatusiText = "Server busy";
                            }
                            else if (transferInfo.Header == Header.PermissionGabim)
                            {
                                transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                                transferi.ProgresiTransferit.StatusiText = "Nuk ka leje";
                            }
                            else if (transferInfo.Header == Header.FileNotFound)
                            {
                                transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                                transferi.ProgresiTransferit.StatusiText = "File nuk ekziston";
                            }
                            else
                            {
                                transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                                transferi.ProgresiTransferit.StatusiText = "Gabim";
                            }
                        }
                        // Gabimet nga kanali komunikues
                        else if (rezultati.Header == Header.PermissionGabim)
                        {
                            transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                            transferi.ProgresiTransferit.StatusiText = "Nuk ka leje";
                        }
                        else if (rezultati.Header == Header.FileNotFound)
                        {
                            transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                            transferi.ProgresiTransferit.StatusiText = "File nuk ekziston";
                        }
                        else
                        {
                            transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                            transferi.ProgresiTransferit.StatusiText = "Gabim";
                        }
                    }
                    else if (transferi.KahuTransferit == KahuTransferit.Ngarkim)
                    {
                        // Be kerkese per upload dhe pranon pergjigjen nga serveri
                        var rezultati = await serverKomunikuesi.KomunikoDyAnshemAsync(new Mesazh(Header.FileUpload, transferi.ProgresiTransferit.FileEmri));

                        if (rezultati.Header == Header.Ok)
                        {
                            // Nese serveri kthen ok, marrim tiketen dhe e dorzojme ne transfer kanal
                            var transferStream = await serverKomunikuesi.HapTransferStreamAsync();
                            var komunikuesi = coreServices.MerrKomunikues();
                            var transferuesi = coreServices.MerrTransferues();
                            transferuesi.PaketDerguar += (s, e) =>
                            {
                                transferi.ProgresiTransferit.Perqindja = (double)e.PaketatTransferuara / e.PaketatTotal;
                            };

                            using (var fileStream = new FileStream(transferi.Identifier, FileMode.Open))
                            {
                                var fileGjatesia = (int)fileStream.Length;
                                await komunikuesi.ShkruajMesazhMeGjatesiAsync(transferStream, new Mesazh(Header.PaHeader, rezultati.TeDhenat), Konfigurimi.TiketGjatesia + fileGjatesia + Konfigurimi.HashGjatesia);
                                var mesazhi = await komunikuesi.LexoMesazhAsync(transferStream);
                                if (mesazhi.Header == Header.Ok)
                                {
                                    transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.DukeTransferuar;
                                    await transferuesi.DergoStreamAsync(fileGjatesia, fileStream, transferStream);
                                }
                                else
                                {
                                    transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                                    transferi.ProgresiTransferit.StatusiText = "Gabim";
                                }
                            }

                            var konfirmimi = await komunikuesi.LexoMesazhAsync(transferStream);
                            if (konfirmimi.Header == Header.Ok)
                            {
                                var fajllInfo = XmlSerializuesi<FajllInfo>.DeserializoBajt(konfirmimi.TeDhenat);
                                if (FajllShtuar != null)
                                {
                                    FajllShtuar(this, new FajllInfoEventArgs(fajllInfo));
                                }

                                transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Perfunduar;
                                transferi.ProgresiTransferit.StatusiText = DateTime.Now.ToString("HH:mm:ss");
                            }
                            else if (konfirmimi.Header == Header.HashFail)
                            {
                                transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                                transferi.ProgresiTransferit.StatusiText = "Hash fail";
                            }
                            else
                            {
                                transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                                transferi.ProgresiTransferit.StatusiText = "Gabim";
                            }
                        }
                        else
                        {
                            transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                            transferi.ProgresiTransferit.StatusiText = "Gabim";
                        }
                    }
                }
                catch (HashFailException)
                {
                    transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                    transferi.ProgresiTransferit.StatusiText = "Hash fail";
                }
                catch
                {
                    transferi.ProgresiTransferit.GjendjaTransferit = GjendjaTransferit.Deshtuar;
                    transferi.ProgresiTransferit.StatusiText = "Gabim";
                }
                finally
                {
                    serverKomunikuesi.MbyllTransferStream();
                }
            }
        }

        public void PastroTransferet()
        {
            for (var i = 0; i < Transferet.Count; i++)
            {
                var transferi = Transferet[i];
                if (transferi.GjendjaTransferit != GjendjaTransferit.DukeTransferuar)
                {
                    Transferet.RemoveAt(i);
                    i--;
                }
            }
        }

        private async void MbajAlive()
        {
            var gabim = false;
            while (startuar)
            {
                try
                {
                    await Task.Delay(Konfigurimi.KeepAliveInterval);
                    var pergjigja = await serverKomunikuesi.KomunikoDyAnshemAsync(new Mesazh(Header.KeepAlive));
                    if (pergjigja.Header != Header.KeepAlive)
                    {
                        gabim = true;
                    }
                }
                catch
                {
                    gabim = true;
                }

                if (gabim)
                {
                    startuar = false;
                    transferSemafori.Release();
                    if (LidhjaHumbur != null)
                    {
                        LidhjaHumbur(this, EventArgs.Empty);
                    }

                    return;
                }
            }
        }

        public async void Dispose()
        {
            if (!startuar)
            {
                return;
            }

            try
            {
                await serverKomunikuesi.DergoMesazhAsync(new Mesazh(Header.Ckycje));
            }
            catch
            {
            }
            finally
            {
                serverKomunikuesi.Dispose();
            }

            startuar = false;
            transferSemafori.Release();
        }
    }
}
