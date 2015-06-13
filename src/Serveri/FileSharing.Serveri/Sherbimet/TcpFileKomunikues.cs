using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FileSharing.Core.Protokoli;
using FileSharing.Core.Protokoli.Sherbimet.Abstrakt;
using FileSharing.Serveri.Sherbimet.Abstrakt;

namespace FileSharing.Serveri.Sherbimet
{
    public class TcpFileKomunikues : IFileKomunikues
    {
        private readonly TcpClient tcpKlienti;
        private readonly NetworkStream tcpStream;

        private readonly int fileGjatesia;

        public TcpFileKomunikues(TcpClient tcpKlienti, NetworkStream tcpStream, IStreamTransferShkruajtes transferuesi, IStreamShkruajtes komunikuesi, int tiketaId, int fileGjatesia)
        {
            TiketaId = tiketaId;
            Transferuesi = transferuesi;
            Komunikuesi = komunikuesi;
            this.fileGjatesia = fileGjatesia;
            this.tcpKlienti = tcpKlienti;
            this.tcpStream = tcpStream;
        }

        public int TiketaId { get; private set; }

        public IStreamTransferShkruajtes Transferuesi { get; set; }

        public IStreamShkruajtes Komunikuesi { get; set; }

        public Task PranoFajllAsync(Stream pranuesi)
        {
            if (fileGjatesia == 0)
            {
                throw new InvalidOperationException();
            }

            return Transferuesi.PranoStreamAsync(fileGjatesia, tcpStream, pranuesi);
        }

        public async Task DergoFajllAsync(byte header, Stream fajlli, int gjatesiaFajllit)
        {
            var gjatesiaTotali = 1 + gjatesiaFajllit;
            await Komunikuesi.ShkruajMesazhMeGjatesiAsync(tcpStream, new Mesazh(header), gjatesiaTotali);
            await Transferuesi.DergoStreamAsync(gjatesiaFajllit, fajlli, tcpStream);
        }

        public Task KthePergjigjeAsync(Mesazh pergjigja)
        {
            return Komunikuesi.ShkruajMesazhAsync(tcpStream, pergjigja);
        }

        public void Dispose()
        {
            tcpStream.Close();
            tcpKlienti.Close();
        }
    }
}
