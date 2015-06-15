using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileSharing.Core;
using FileSharing.Core.Protokoli;
using FileSharing.Klienti.Modeli;
using FileSharing.Klienti.Sherbimet.Abstrakt;

namespace FileSharing.Klienti.Sherbimet
{
    public class TcpServerKomunikues : IServerKomunikues
    {
        private readonly SemaphoreSlim komunikimiSemafori = new SemaphoreSlim(1, 1);

        private readonly TcpClient klienti;

        private readonly NetworkStream tcpStream;

        private readonly ICoreServiceLocator coreSherbimet;

        private readonly IAddressMarres addressMarresi;

        private TcpClient transferKlienti;

        private NetworkStream transferStream;

        private bool channelHapur;

        public TcpServerKomunikues(TcpClient klienti, NetworkStream tcpStream, IAddressMarres addressMarresi, ICoreServiceLocator coreSherbimet)
        {
            this.klienti = klienti;
            this.tcpStream = tcpStream;
            this.addressMarresi = addressMarresi;
            this.coreSherbimet = coreSherbimet;
        }

        public async Task DergoMesazhAsync(Mesazh mesazhi)
        {
            await komunikimiSemafori.WaitAsync();
            try
            {
                await coreSherbimet.MerrKomunikues().ShkruajMesazhAsync(tcpStream, mesazhi);
            }
            finally
            {
                komunikimiSemafori.Release();
            }
        }

        public async Task<Mesazh> PranoMesazhAsync()
        {
            await komunikimiSemafori.WaitAsync();
            try
            {
                var mesazhi = await coreSherbimet.MerrKomunikues().LexoMesazhAsync(tcpStream);
                return mesazhi;
            }
            finally
            {
                komunikimiSemafori.Release();
            }
        }

        public async Task<Mesazh> KomunikoDyAnshemAsync(Mesazh mesazhi)
        {
            await komunikimiSemafori.WaitAsync();
            try
            {
                var komunikuesi = coreSherbimet.MerrKomunikues();
                await komunikuesi.ShkruajMesazhAsync(tcpStream, mesazhi);
                var pergjigja = await komunikuesi.LexoMesazhAsync(tcpStream);
                return pergjigja;
            }
            finally
            {
                komunikimiSemafori.Release();
            }
        }

        public async Task<Stream> HapTransferStreamAsync()
        {
            if (channelHapur)
            {
                throw new InvalidOperationException();
            }

            transferKlienti = new TcpClient();
            await transferKlienti.ConnectAsync(addressMarresi.ServerAdresa, addressMarresi.PortiTransferit);
            channelHapur = true;
            return transferStream = transferKlienti.GetStream();
        }

        public async Task<TransferInfo> MerrTransferInfoAsync()
        {
            if (!channelHapur)
            {
                throw new InvalidOperationException();
            }

            var lexuar = 0;
            var gjatesiaBuffer = new byte[Konfigurimi.PrefixGjatesia];
            while (lexuar < Konfigurimi.PrefixGjatesia)
            {
                var delta = await transferStream.ReadAsync(gjatesiaBuffer, 0, Konfigurimi.PrefixGjatesia - lexuar);
                if (delta == 0)
                {
                    throw new InvalidOperationException();
                }
                
                lexuar += delta;
            }

            var gjatesiaFajllit = BitConverter.ToInt32(gjatesiaBuffer, 0) - 1;
            var headerBuffer = new byte[1];
            lexuar = await transferStream.ReadAsync(headerBuffer, 0, 1);
            if (lexuar == 0)
            {
                throw new InvalidOperationException();
            }

            return new TransferInfo(headerBuffer[0], gjatesiaFajllit);
        }

        public void MbyllTransferStream()
        {
            if (channelHapur)
            {
                transferStream.Close();
                transferKlienti.Close();
            }

            channelHapur = false;
        }

        public void Dispose()
        {
            tcpStream.Close();
            klienti.Close();
        }
    }
}
