using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
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
        private readonly string serverEmri;

        private readonly SemaphoreSlim transferetSemafori;
        private readonly Random random;
        private bool startuar;

        private readonly byte[] iv;
        private readonly byte[] key;

        public Server(string emri, string password, IRepository repository, IPathResolver pathat, IServerServiceLocator serviceLocator,
            int maxTransfere)
        {
            serverEmri = emri;
            Repository = repository;
            PathResolver = pathat;
            transferetSemafori = new SemaphoreSlim(maxTransfere);
            random = new Random();
            KlientPranuesi = serviceLocator.MerrKlientPranues();
            TransferPranuesi = serviceLocator.MerrFilePranues();
            KlientetLoguar = new Dictionary<string, IKlientKomunikues>(StringComparer.OrdinalIgnoreCase);
            Transferet = new Dictionary<int, TransferTikete>();
            using (var keyGen = new Rfc2898DeriveBytes(password, new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }))
            {
                iv = keyGen.GetBytes(16);
                key = keyGen.GetBytes(16);
            }
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
                var klienti = await KlientPranuesi.PranoKlientAsync();
                Task.Run(() => BisedoMeKlient(klienti));
            }
        }

        private async void BisedoMeKlient(IKlientKomunikues klienti)
        {
            // Provo identifiko
            Shfrytezues shfrytezuesi;
            try
            {
                var identifikimi = await klienti.PranoAsync();

                if (identifikimi.Header == Header.KrijoUser)
                {
                    shfrytezuesi = await ProvoParse<Shfrytezues>(klienti, identifikimi.TeDhenat);
                    if (shfrytezuesi != null)
                    {
                        var krijuar = Repository.KrijoUser(shfrytezuesi.Emri, shfrytezuesi.Fjalekalimi);
                        if (krijuar)
                        {
                            await klienti.DergoAsync(new Mesazh(Header.Ok));
                        }
                        else
                        {
                            await klienti.DergoAsync(new Mesazh(Header.Gabim));
                        }
                    }

                    klienti.Dispose();
                    return;
                }

                if (identifikimi.Header != Header.Identifikim)
                {
                    await klienti.DergoAsync(new Mesazh(Header.IdentifikimGabim));
                    klienti.Dispose();
                    return;
                }

                shfrytezuesi = await ProvoParse<Shfrytezues>(klienti, identifikimi.TeDhenat);
                if (shfrytezuesi != null)
                {
                    var loginOk = Repository.TestoLogin(shfrytezuesi.Emri, shfrytezuesi.Fjalekalimi);
                    if (loginOk)
                    {
                        if (KlientetLoguar.ContainsKey(shfrytezuesi.Emri))
                        {
                            await klienti.DergoAsync(new Mesazh(Header.UserLoguarGabim, serverEmri));
                            klienti.Dispose();
                            return;
                        }

                        await klienti.DergoAsync(new Mesazh(Header.Ok, serverEmri));
                        KlientetLoguar[shfrytezuesi.Emri] = klienti;
                    }
                    else
                    {
                        await klienti.DergoAsync(new Mesazh(Header.InvalidUserOsePass));
                        klienti.Dispose();
                        return;
                    }
                }
                else
                {
                    klienti.Dispose();
                    return;
                }
            }
            catch
            {
                klienti.Dispose();
                return;
            }

            var lejetEkstra = new HashSet<int>();
            while (startuar)
            {
                try
                {
                    var kerkesa = await klienti.PranoAsync();
                    switch (kerkesa.Header)
                    {
                        case Header.KeepAlive:
                            {
                                await klienti.DergoAsync(new Mesazh(Header.KeepAlive));
                                break;
                            }

                        case Header.Ckycje:
                            {
                                KlientetLoguar.Remove(shfrytezuesi.Emri);
                                klienti.Dispose();
                                return;
                            }

                        case Header.MerrFajllat:
                            {
                                var fajllat = Repository.MerrFajllatUserit(shfrytezuesi.Emri);
                                var fajllatSerializuar = XmlSerializuesi<FajllInfo[]>.SerializoBajt(fajllat);
                                await klienti.DergoAsync(new Mesazh(Header.Ok, fajllatSerializuar));
                                break;
                            }

                        case Header.FileDownload:
                            {
                                int idFile;
                                if (IntegerKonvertuesi.ProvoNgaBajtat(kerkesa.TeDhenat, out idFile))
                                {
                                    var fajlli = Repository.MerrFajllInfo(idFile);
                                    if (fajlli == null)
                                    {
                                        await klienti.DergoAsync(new Mesazh(Header.FileNotFound));
                                    }
                                    else if (fajlli.Dukshmeria == Dukshmeria.Private && !String.Equals(fajlli.Pronari, shfrytezuesi.Emri, StringComparison.OrdinalIgnoreCase) && !lejetEkstra.Contains(idFile))
                                    {
                                        await klienti.DergoAsync(new Mesazh(Header.PermissionGabim));
                                    }
                                    else
                                    {
                                        // Fajlli ekziston & ka permission
                                        int tiketaId;
                                        do
                                        {
                                            tiketaId = random.Next();
                                        } while (Transferet.ContainsKey(tiketaId));

                                        Transferet[tiketaId] = new TransferTikete
                                        {
                                            // Krijon tikete per shkarkim qe do te konsumohet ne transfer kanal
                                            Kahu = KahuTransferit.Shkarkim,
                                            Fajlli = fajlli,
                                            KohaKerkeses = DateTime.Now,
                                            // DataSkadimit = ... per t'u implementuar
                                        };

                                        await
                                            klienti.DergoAsync(new Mesazh(Header.Ok, IntegerKonvertuesi.NeBajta(tiketaId)));
                                    }
                                }
                                else
                                {
                                    await klienti.DergoAsync(new Mesazh(Header.ParseGabim));
                                }

                                break;
                            }

                        case Header.FileUpload:
                            {
                                var emri = kerkesa.Teksti;
                                if (string.IsNullOrEmpty(emri))
                                {
                                    await klienti.DergoAsync(new Mesazh(Header.ParseGabim));
                                }
                                else
                                {
                                    var fajllInfo = new FajllInfo
                                    {
                                        Emri = emri,
                                        Pronari = shfrytezuesi.Emri
                                    };

                                    int tiketaId;
                                    do
                                    {
                                        tiketaId = random.Next();
                                    } while (Transferet.ContainsKey(tiketaId));

                                    Transferet[tiketaId] = new TransferTikete
                                    {
                                        // Tikete per upload
                                        Kahu = KahuTransferit.Ngarkim,
                                        Fajlli = fajllInfo,
                                        KohaKerkeses = DateTime.Now,
                                        // DataSkadimit = ... per t'u implementuar
                                    };

                                    await klienti.DergoAsync(new Mesazh(Header.Ok, IntegerKonvertuesi.NeBajta(tiketaId)));
                                }

                                break;
                            }

                        case Header.FileDelete:
                            {
                                int idFile;
                                if (IntegerKonvertuesi.ProvoNgaBajtat(kerkesa.TeDhenat, out idFile))
                                {
                                    var fajllInfo = Repository.MerrFajllInfo(idFile);
                                    if (fajllInfo == null)
                                    {
                                        await klienti.DergoAsync(new Mesazh(Header.FileNotFound));
                                    }
                                    else if (
                                        !String.Equals(fajllInfo.Pronari, shfrytezuesi.Emri,
                                            StringComparison.OrdinalIgnoreCase))
                                    {
                                        await klienti.DergoAsync(new Mesazh(Header.PermissionGabim));
                                    }
                                    else
                                    {
                                        var filePath = PathResolver.GetFileInDataPath(fajllInfo.Id.ToString());
                                        if (File.Exists(filePath))
                                        {
                                            bool fshire;
                                            try
                                            {
                                                File.Delete(filePath);
                                                fshire = true;
                                            }
                                            catch
                                            {
                                                fshire = false;
                                            }

                                            if (!fshire)
                                            {
                                                await klienti.DergoAsync(new Mesazh(Header.Gabim));
                                                break;
                                            }
                                        }

                                        if (Repository.DeleteFajll(fajllInfo))
                                        {
                                            await klienti.DergoAsync(new Mesazh(Header.Ok));
                                        }
                                        else
                                        {
                                            await klienti.DergoAsync(new Mesazh(Header.Gabim));
                                        }
                                    }
                                }
                                else
                                {
                                    await klienti.DergoAsync(new Mesazh(Header.ParseGabim));
                                }
                                break;
                            }

                        case Header.Search:
                            {
                                var termi = kerkesa.Teksti.Trim();
                                if (termi.Length == 0)
                                {
                                    await klienti.DergoAsync(new Mesazh(Header.Gabim));
                                    break;
                                }

                                if (termi.Length == 32)
                                {
                                    try
                                    {
                                        var termiBajtat = StringToByteArray(termi);
                                        using (var aes = new AesManaged { Padding = PaddingMode.None })
                                        using (var decryptor = aes.CreateDecryptor(key, iv))
                                        using (var memoryStream = new MemoryStream(termiBajtat))
                                        using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                                        {
                                            var buffer = new byte[4];
                                            cryptoStream.Read(buffer, 0, 4);
                                            var id = BitConverter.ToInt32(buffer, 0);
                                            var fajlli = Repository.MerrFajllInfo(id);
                                            if (fajlli != null)
                                            {
                                                var emriBuffer = new byte[12];
                                                cryptoStream.Read(emriBuffer, 0, 12);
                                                var pronari = Encoding.UTF8.GetBytes(fajlli.Pronari);
                                                var max = pronari.Length > 12 ? 12 : pronari.Length;
                                                var baraz = true;
                                                for (var i = 0; i < max; i++)
                                                {
                                                    if (emriBuffer[i] != pronari[i])
                                                    {
                                                        baraz = false;
                                                        break;
                                                    }
                                                }

                                                if (baraz)
                                                {
                                                    // Kodi valid, shto id dhe kthe fajll
                                                    if (!String.Equals(fajlli.Pronari, shfrytezuesi.Emri, StringComparison.OrdinalIgnoreCase))
                                                    {
                                                        lejetEkstra.Add(id);
                                                    }

                                                    var rezultati = new[]
                                                    {
                                                        new RezultatKerkimi
                                                        {
                                                            Emri = "1 Fajll",
                                                            Fajllat = new FajllInfo[] { fajlli },
                                                            LlojiRezultatit = LlojiRezultatit.Fajll
                                                        }
                                                    };

                                                    await klienti.DergoAsync(new Mesazh(Header.Ok,
                                                            XmlSerializuesi<RezultatKerkimi[]>.SerializoBajt(rezultati)));
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    catch { }
                                }

                                var rezultatet = Repository.Kerko(shfrytezuesi.Emri, termi);
                                await klienti.DergoAsync(new Mesazh(Header.Ok,
                                    XmlSerializuesi<RezultatKerkimi[]>.SerializoBajt(rezultatet)));
                                break;
                            }

                        case Header.BejePublik:
                        case Header.BejePrivat:
                            {
                                int idFile;
                                if (IntegerKonvertuesi.ProvoNgaBajtat(kerkesa.TeDhenat, out idFile))
                                {
                                    var fajllInfo = Repository.MerrFajllInfo(idFile);
                                    if (fajllInfo == null)
                                    {
                                        await klienti.DergoAsync(new Mesazh(Header.FileNotFound));
                                    }
                                    else if (
                                        !String.Equals(fajllInfo.Pronari, shfrytezuesi.Emri,
                                            StringComparison.OrdinalIgnoreCase))
                                    {
                                        await klienti.DergoAsync(new Mesazh(Header.PermissionGabim));
                                    }
                                    else
                                    {
                                        fajllInfo.Dukshmeria = kerkesa.Header == Header.BejePublik
                                            ? Dukshmeria.Publike
                                            : Dukshmeria.Private;

                                        if (Repository.UpdateFajll(fajllInfo))
                                        {
                                            await klienti.DergoAsync(new Mesazh(Header.Ok));
                                        }
                                        else
                                        {
                                            await klienti.DergoAsync(new Mesazh(Header.Gabim));
                                        }
                                    }
                                }
                                else
                                {
                                    await klienti.DergoAsync(new Mesazh(Header.ParseGabim));
                                }

                                break;
                            }

                        case Header.MerrLink:
                            {
                                int idFile;
                                if (IntegerKonvertuesi.ProvoNgaBajtat(kerkesa.TeDhenat, out idFile))
                                {
                                    var fajllInfo = Repository.MerrFajllInfo(idFile);
                                    if (fajllInfo == null)
                                    {
                                        await klienti.DergoAsync(new Mesazh(Header.FileNotFound));
                                    }
                                    else if (fajllInfo.Dukshmeria == Dukshmeria.Private && !String.Equals(fajllInfo.Pronari, shfrytezuesi.Emri, StringComparison.OrdinalIgnoreCase) && !lejetEkstra.Contains(idFile))
                                    {
                                        await klienti.DergoAsync(new Mesazh(Header.PermissionGabim));
                                    }
                                    else
                                    {
                                        Mesazh pergjigja;
                                        try
                                        {
                                            using (var aes = new AesManaged { Padding = PaddingMode.None })
                                            using (var encryptor = aes.CreateEncryptor(key, iv))
                                            using (var memoryStream = new MemoryStream())
                                            {
                                                using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                                                {
                                                    var buffer = new byte[16];
                                                    var emriBytes = Encoding.UTF8.GetBytes(fajllInfo.Pronari);
                                                    if (emriBytes.Length < 12)
                                                    {
                                                        Array.Resize(ref emriBytes, 12);
                                                    }

                                                    Buffer.BlockCopy(kerkesa.TeDhenat, 0, buffer, 0, 4);
                                                    Buffer.BlockCopy(emriBytes, 0, buffer, 4, 12);
                                                    cryptoStream.Write(buffer, 0, 16);
                                                }

                                                pergjigja = new Mesazh(Header.Ok, BitConverter.ToString(memoryStream.ToArray()).Replace("-", ""));
                                            }
                                        }
                                        catch
                                        {
                                            pergjigja = new Mesazh(Header.Gabim);
                                        }

                                        await klienti.DergoAsync(pergjigja);
                                    }
                                }
                                else
                                {
                                    await klienti.DergoAsync(new Mesazh(Header.ParseGabim));
                                }

                                break;
                            }

                        default:
                            {
                                await klienti.DergoAsync(new Mesazh(Header.Gabim));
                                break;
                            }
                    }
                }
                catch
                {
                    KlientetLoguar.Remove(shfrytezuesi.Emri);
                    klienti.Dispose();
                    return;
                }
            }

            klienti.Dispose();
        }

        /// <summary>
        /// Provon ta deserializoje objektin, nese nuk mundet i kthen mesazh gabimi klientit.
        /// </summary>
        private static async Task<T> ProvoParse<T>(IKlientKomunikues klienti, byte[] objektiBajtat) where T : class
        {
            T objekti = null;
            try
            {
                objekti = XmlSerializuesi<T>.DeserializoBajt(objektiBajtat);
            }
            catch
            {
            }

            if (objekti == null)
            {
                await klienti.DergoAsync(new Mesazh(Header.ParseGabim));
            }

            return objekti;
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
                var busy = !await transferetSemafori.WaitAsync(500);
                if (busy)
                {
                    await fileKerkesa.KthePergjigjeAsync(new Mesazh(Header.ServerBusy));
                    fileKerkesa.Dispose();
                    continue;
                }

                var fajlli = tiketa.Fajlli;
                if (tiketa.Kahu == KahuTransferit.Ngarkim)
                {
                    await fileKerkesa.KthePergjigjeAsync(new Mesazh(Header.Ok));
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

                            fajlli.Madhesia = (int)fileStream.Length;
                            fileStream.Close();

                            fajlli.Dukshmeria = Dukshmeria.Private;
                            fajlli.DataShtimit = DateTime.Now;
                            Repository.ShtoFajll(fajlli);


                            // Fajlli sukses, dergoje ne folder te vertet
                            File.Move(tempFile, PathResolver.GetFileInDataPath(fajlli.Id.ToString()));
                            pergjigja = new Mesazh(Header.Ok, XmlSerializuesi<FajllInfo>.Serializo(fajlli));
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
                            using (var fileStream = new FileStream(filePath, FileMode.Open))
                            {
                                await fileKerkesa.DergoFajllAsync(Header.Ok, fileStream, (int)fileStream.Length);
                            }
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

        public static byte[] StringToByteArray(string hex)
        {
            if (hex.Length % 2 == 1)
                throw new Exception("The binary key cannot have an odd number of digits");

            var arr = new byte[hex.Length >> 1];

            for (var i = 0; i < hex.Length >> 1; ++i)
            {
                arr[i] = (byte)((GetHexVal(hex[i << 1]) << 4) + (GetHexVal(hex[(i << 1) + 1])));
            }

            return arr;
        }

        private static int GetHexVal(char hex)
        {
            var val = (int)hex;
            return val - (val < 58 ? 48 : (val < 97 ? 55 : 87));
        }
    }
}

#pragma warning restore 4014