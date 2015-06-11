using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using FileSharing.Core;
using FileSharing.Serveri.Sherbimet.Abstrakt;

namespace FileSharing.Serveri.Sherbimet
{
    class TcpKlientPranues : IKlientPranues
    {
        private readonly ICoreServiceLocator coreServiceLocator;

        private bool startuar;

        public TcpKlientPranues(TcpListener tcpListener, ICoreServiceLocator coreServiceLocator)
        {
            this.coreServiceLocator = coreServiceLocator;
            TcpListener = tcpListener;
        }

        public TcpListener TcpListener { get; private set; }

        public async Task<IKlientKomunikues> PranoKlientAsync()
        {
            if (!startuar)
            {
                throw new InvalidOperationException();
            }

            var klienti = await TcpListener.AcceptTcpClientAsync();
            return new TcpKlientKomunikues(klienti, klienti.GetStream(), coreServiceLocator.MerrKomunikues());
        }

        public void Starto()
        {
            if (!startuar)
            {
                TcpListener.Start();
                startuar = true;
            }
        }

        public void Ndalo()
        {
            if (startuar)
            {
                TcpListener.Stop();
                startuar = false;
            }
        }
    }
}
