using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using FileSharing.Core.Protokoli;
using FileSharing.Core.Protokoli.Sherbimet.Abstrakt;
using FileSharing.Serveri.Sherbimet.Abstrakt;

namespace FileSharing.Serveri.Sherbimet
{
    public class TcpKlientKomunikues : IKlientKomunikues
    {
        private readonly TcpClient tcpKlienti;
        private readonly NetworkStream tcpStream;

        public TcpKlientKomunikues(TcpClient tcpKlienti, NetworkStream tcpStream, IStreamShkruajtes komunikuesi)
        {
            this.tcpKlienti = tcpKlienti;
            this.tcpStream = tcpStream;
            Komunikuesi = komunikuesi;
        }

        public IStreamShkruajtes Komunikuesi { get; set; }

        public Task<Mesazh> PranoAsync()
        {
            return Komunikuesi.LexoMesazhAsync(tcpStream);
        }

        public Task DergoAsync(Mesazh mesazhi)
        {
            return Komunikuesi.ShkruajMesazhAsync(tcpStream, mesazhi);
        }

        public void Dispose()
        {
            tcpStream.Close();
            tcpKlienti.Close();
        }
    }
}
