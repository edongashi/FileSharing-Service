using System;
using System.IO;
using System.Threading.Tasks;
using Shared.Protokoli.Sherbimet.Abstrakt;

namespace Shared.Protokoli.Sherbimet
{
    class StreamTransferShkruajtes : IStreamShkruajtes
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
            //var mesazhiGjatesia = 
            //streami.ReadAsync()
        }
    }
}
