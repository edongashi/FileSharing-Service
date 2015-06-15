using System;
using System.IO;
using System.Threading.Tasks;
using FileSharing.Core.Protokoli.Sherbimet.Abstrakt;

namespace FileSharing.Core.Protokoli.Sherbimet
{
    public class StreamShkruajtes : IStreamShkruajtes
    {
        public Task ShkruajMesazhAsync(Stream pranuesi, Mesazh mesazhi)
        {
            var mesazhiGjatesia = mesazhi.Gjatesia;
            var bufferi = new byte[Konfigurimi.PrefixGjatesia + mesazhiGjatesia];
            if (mesazhi.Header != Header.PaHeader)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(mesazhiGjatesia), 0, bufferi, 0, Konfigurimi.PrefixGjatesia);
                bufferi[Konfigurimi.PrefixGjatesia] = mesazhi.Header;
                Buffer.BlockCopy(mesazhi.TeDhenat, 0, bufferi, Konfigurimi.PrefixGjatesia + 1, mesazhi.TeDhenat.Length);
            }
            else
            {
                Buffer.BlockCopy(BitConverter.GetBytes(mesazhiGjatesia), 0, bufferi, 0, Konfigurimi.PrefixGjatesia);
                Buffer.BlockCopy(mesazhi.TeDhenat, 0, bufferi, Konfigurimi.PrefixGjatesia, mesazhi.TeDhenat.Length);
            }

            return pranuesi.WriteAsync(bufferi, 0, bufferi.Length);
        }

        public Task ShkruajMesazhMeGjatesiAsync(Stream pranuesi, Mesazh mesazhi, int gjatesia)
        {
            if (gjatesia < 1)
            {
                throw new InvalidOperationException();
            }

            var mesazhiGjatesia = mesazhi.Gjatesia;
            var bufferi = new byte[Konfigurimi.PrefixGjatesia + mesazhiGjatesia];
            if (mesazhi.Header != Header.PaHeader)
            {
                Buffer.BlockCopy(BitConverter.GetBytes(gjatesia), 0, bufferi, 0, Konfigurimi.PrefixGjatesia);
                bufferi[Konfigurimi.PrefixGjatesia] = mesazhi.Header;
                Buffer.BlockCopy(mesazhi.TeDhenat, 0, bufferi, Konfigurimi.PrefixGjatesia + 1, mesazhi.TeDhenat.Length);
            }
            else
            {
                Buffer.BlockCopy(BitConverter.GetBytes(gjatesia), 0, bufferi, 0, Konfigurimi.PrefixGjatesia);
                Buffer.BlockCopy(mesazhi.TeDhenat, 0, bufferi, Konfigurimi.PrefixGjatesia, mesazhi.TeDhenat.Length);
            }

            return pranuesi.WriteAsync(bufferi, 0, bufferi.Length);
        }

        public async Task<Mesazh> LexoMesazhAsync(Stream derguesi)
        {
            var lexuar = 0;
            var mesazhiGjatesiaBuffer = new byte[Konfigurimi.PrefixGjatesia];
            while (lexuar < Konfigurimi.PrefixGjatesia)
            {
                var delta = await derguesi.ReadAsync(mesazhiGjatesiaBuffer, lexuar, Konfigurimi.PrefixGjatesia - lexuar);
                if (delta == 0)
                {
                    throw new InvalidOperationException();
                }

                lexuar += delta;
            }

            var mesazhiGjatesia = BitConverter.ToInt32(mesazhiGjatesiaBuffer, 0);
            if (mesazhiGjatesia < 0)
            {
                throw new PrefixException();
            }

            if (mesazhiGjatesia == 0)
            {
                return new Mesazh(Header.PaHeader);
            }

            lexuar = 0;
            var bufferi = new byte[mesazhiGjatesia];
            while (lexuar < mesazhiGjatesia)
            {
                var delta = await derguesi.ReadAsync(bufferi, lexuar, mesazhiGjatesia - lexuar);
                if (delta == 0)
                {
                    throw new InvalidOperationException();
                }

                lexuar += delta;
            }

            var header = bufferi[0];
            var teDhenat = new byte[mesazhiGjatesia - 1];
            Buffer.BlockCopy(bufferi, 1, teDhenat, 0, teDhenat.Length);
            return new Mesazh(header, teDhenat);
        }
    }
}
