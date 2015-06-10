using System;
using System.IO;
using System.Threading.Tasks;
using FileSharing.Core.Protokoli;
using Shared.Protokoli.Sherbimet.Abstrakt;

namespace Shared.Protokoli.Sherbimet
{
    class StreamShkruajtes : IStreamShkruajtes
    {
        public Task ShkruajMesazhAsync(Stream streami, Mesazh mesazhi)
        {
            var mesazhiGjatesia = mesazhi.TeDhenat.Length + 1;
            var bufferi = new byte[Konfigurimi.PrefixGjatesia + mesazhiGjatesia];
            Buffer.BlockCopy(BitConverter.GetBytes(mesazhiGjatesia), 0, bufferi, 0, Konfigurimi.PrefixGjatesia);
            bufferi[Konfigurimi.PrefixGjatesia] = mesazhi.Header;
            Buffer.BlockCopy(mesazhi.TeDhenat, 0, bufferi, mesazhiGjatesia, mesazhi.TeDhenat.Length);
            return streami.WriteAsync(bufferi, 0, bufferi.Length);
        }

        public async Task<Mesazh> LexoMesazhAsync(Stream streami)
        {
            var mesazhiGjatesiaBuffer = new byte[Konfigurimi.PrefixGjatesia];
            await streami.ReadAsync(mesazhiGjatesiaBuffer, 0, Konfigurimi.PrefixGjatesia);
            var mesazhiGjatesia = BitConverter.ToInt32(mesazhiGjatesiaBuffer, 0);
            if (mesazhiGjatesia < 0)
            {
                throw new PrefixException();
            }

            if (mesazhiGjatesia == 0)
            {
                return new Mesazh(Header.PaHeader);
            }

            var bufferi = new byte[mesazhiGjatesia];
            await streami.ReadAsync(bufferi, 0, mesazhiGjatesia);
            var header = bufferi[0];
            var teDhenat = new byte[mesazhiGjatesia - 1];
            Buffer.BlockCopy(bufferi, 1, teDhenat, 0, teDhenat.Length);
            return new Mesazh(header, teDhenat);
        }
    }
}
