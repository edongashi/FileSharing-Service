using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FileSharing.Core.Protokoli.Sherbimet.Abstrakt;

namespace FileSharing.Core.Protokoli.Sherbimet
{
    public class StreamTransferShkruajtes : IStreamTransferShkruajtes
    {
        public event EventHandler<PaketTransferuarEventArgs> PaketDerguar;

        public event EventHandler<PaketTransferuarEventArgs> PaketPranuar;

        /// <summary>
        /// Transferon te dhenat nga derguesi tek pranuesi ne madhesi fikse te paketave.
        /// Ne fund bashkangjet hash-in e te dhenave te derguara.
        /// </summary>
        /// <param name="gjatesia">
        /// Numri i bajtave qe do te lexohen nga derguesi.
        /// Nuk perfshine gjatesine e hash funksioniet.
        /// </param>
        /// <param name="derguesi">Stream nga ku do te merren te dhenat.</param>
        /// <param name="pranuesi">Stream ku do te dergohen te dhenat.</param>
        public async Task DergoStreamAsync(int gjatesia, Stream derguesi, Stream pranuesi)
        {
            var numriPaketavePlota = gjatesia / Konfigurimi.PaketMadhesia;
            var teprica = gjatesia % Konfigurimi.PaketMadhesia;
            var totalPaketa = numriPaketavePlota + 1 + teprica > 0 ? 1 : 0;
            
            var hashAlgoritmi = HashAlgorithm.Create(Konfigurimi.HashAlgoritmi);

            var buffer = new byte[Konfigurimi.PaketMadhesia];
            var lexuar = 0;
            for (var i = 0; i < numriPaketavePlota; i++)
            {
                while (lexuar < Konfigurimi.PaketMadhesia)
                {
                    var delta = await derguesi.ReadAsync(buffer, lexuar, Konfigurimi.PaketMadhesia - lexuar);
                    if (delta == 0)
                    {
                        throw new InvalidOperationException();
                    }

                    lexuar += delta;
                }

                await pranuesi.WriteAsync(buffer, 0, Konfigurimi.PaketMadhesia);
                // ReSharper disable once PossibleNullReferenceException
                hashAlgoritmi.TransformBlock(buffer, 0, Konfigurimi.PaketMadhesia, null, 0);

                if (PaketDerguar != null)
                {
                    PaketDerguar(this, new PaketTransferuarEventArgs(i, totalPaketa));
                }

                lexuar = 0;
            }

            if (teprica != 0)
            {
                while (lexuar < teprica)
                {
                    var delta = await derguesi.ReadAsync(buffer, lexuar, teprica - lexuar);
                    if (delta == 0)
                    {
                        throw new InvalidOperationException();
                    }

                    lexuar += delta;
                }

                await pranuesi.WriteAsync(buffer, 0, teprica);
                // ReSharper disable once PossibleNullReferenceException
                hashAlgoritmi.TransformFinalBlock(buffer, 0, teprica);
                if (PaketDerguar != null)
                {
                    PaketDerguar(this, new PaketTransferuarEventArgs(totalPaketa - 1, totalPaketa));
                }
            }
            else
            {
                // ReSharper disable once PossibleNullReferenceException
                hashAlgoritmi.TransformFinalBlock(buffer, 0, 0);
            }

            var hash = hashAlgoritmi.Hash;
            await pranuesi.WriteAsync(hash, 0, hash.Length);
            if (PaketDerguar != null)
            {
                PaketDerguar(this, new PaketTransferuarEventArgs(totalPaketa, totalPaketa));
            }
        }

        // Ne kete rast parametri perfshin gjatesine e hash-it.
        public async Task PranoStreamAsync(int gjatesia, Stream derguesi, Stream pranuesi)
        {
            var hashAlgoritmi = HashAlgorithm.Create(Konfigurimi.HashAlgoritmi);
            // ReSharper disable once PossibleNullReferenceException
            var hashGjatesia = hashAlgoritmi.HashSize / 8;

            if (gjatesia <= hashGjatesia)
            {
                throw new ArgumentException("Gjatesia e leximit me e vogel se gjatesia e hash funksionit.");
            }

            var gjatesiaPaHash = gjatesia - hashGjatesia;

            var numriPaketavePlota = gjatesiaPaHash / Konfigurimi.PaketMadhesia;
            var teprica = gjatesiaPaHash % Konfigurimi.PaketMadhesia;
            var totalPaketa = numriPaketavePlota + 1 + teprica > 0 ? 1 : 0;

            var buffer = new byte[Konfigurimi.PaketMadhesia];
            var lexuar = 0;
            for (var i = 0; i < numriPaketavePlota; i++)
            {
                while (lexuar < Konfigurimi.PaketMadhesia)
                {
                    var delta = await derguesi.ReadAsync(buffer, lexuar, Konfigurimi.PaketMadhesia - lexuar);
                    if (delta == 0)
                    {
                        throw new InvalidOperationException();
                    }

                    lexuar += delta;
                }

                await pranuesi.WriteAsync(buffer, 0, Konfigurimi.PaketMadhesia);
                // ReSharper disable once PossibleNullReferenceException
                hashAlgoritmi.TransformBlock(buffer, 0, Konfigurimi.PaketMadhesia, null, 0);

                if (PaketPranuar != null)
                {
                    PaketPranuar(this, new PaketTransferuarEventArgs(i, totalPaketa));
                }

                lexuar = 0;
            }

            if (teprica != 0)
            {
                while (lexuar < teprica)
                {
                    var delta = await derguesi.ReadAsync(buffer, lexuar, teprica - lexuar);
                    if (delta == 0)
                    {
                        throw new InvalidOperationException();
                    }

                    lexuar += delta;
                }

                await pranuesi.WriteAsync(buffer, 0, teprica);
                // ReSharper disable once PossibleNullReferenceException
                hashAlgoritmi.TransformFinalBlock(buffer, 0, teprica);
                if (PaketDerguar != null)
                {
                    PaketDerguar(this, new PaketTransferuarEventArgs(totalPaketa - 1, totalPaketa));
                }
            }
            else
            {
                // ReSharper disable once PossibleNullReferenceException
                hashAlgoritmi.TransformFinalBlock(buffer, 0, 0);
            }

            var hashLlogaritur = hashAlgoritmi.Hash;
            var hashPranuar = new byte[hashGjatesia];
            lexuar = 0;
            while (lexuar < hashGjatesia)
            {
                var delta = await derguesi.ReadAsync(hashPranuar, lexuar, hashGjatesia - lexuar);
                if (delta == 0)
                {
                    throw new InvalidOperationException();
                }

                lexuar += delta;
            }
            if (!hashLlogaritur.SequenceEqual(hashPranuar))
            {
                throw new HashFailException();
            }

            if (PaketDerguar != null)
            {
                PaketDerguar(this, new PaketTransferuarEventArgs(totalPaketa, totalPaketa));
            }
        }
    }
}
