using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using FileSharing.Core;
using FileSharing.Core.Protokoli;
using FileSharing.Serveri.Sherbimet.Abstrakt;

namespace FileSharing.Serveri.Sherbimet
{
    public class TcpFileKomunikuesPranues : IFileKomunikuesPranues
    {
        private readonly ICoreServiceLocator coreServiceLocator;

        private bool startuar;

        public TcpFileKomunikuesPranues(TcpListener tcpListener, ICoreServiceLocator coreServiceLocator)
        {
            this.coreServiceLocator = coreServiceLocator;
            TcpListener = tcpListener;
        }

        public TcpListener TcpListener { get; private set; }

        public async Task<IFileKomunikues> PranoFileKomunikuesAsync()
        {
            while (startuar)
            {
                var klienti = await TcpListener.AcceptTcpClientAsync();
                var tcpStream = klienti.GetStream();
                var gjatesiaPrefix = new byte[Konfigurimi.PrefixGjatesia];
                var lexuar = 0;
                while (lexuar < Konfigurimi.PrefixGjatesia)
                {
                    var delta = await tcpStream.ReadAsync(gjatesiaPrefix, lexuar, Konfigurimi.PrefixGjatesia - lexuar);

                    if (delta == 0)
                    {
                        try
                        {
                            tcpStream.Dispose();
                            klienti.Close();
                        }
                        catch { }
                        continue;
                    }

                    lexuar += delta;
                }

                var mesazhiGjatesia = BitConverter.ToInt32(gjatesiaPrefix, 0);
                if (mesazhiGjatesia < Konfigurimi.TiketGjatesia)
                {
                    try
                    {
                        tcpStream.Dispose();
                        klienti.Close();
                    }
                    catch { }
                    continue;
                }

                lexuar = 0;
                var ticketBuffer = new byte[Konfigurimi.TiketGjatesia];
                while (lexuar < Konfigurimi.TiketGjatesia)
                {
                    var delta = await tcpStream.ReadAsync(gjatesiaPrefix, lexuar, Konfigurimi.TiketGjatesia - lexuar);

                    if (delta == 0)
                    {
                        try
                        {
                            tcpStream.Dispose();
                            klienti.Close();
                        }
                        catch { }
                        continue;
                    }

                    lexuar += delta;
                }

                var tiketa = BitConverter.ToInt32(ticketBuffer, 0);
                var fajllGjatesia = mesazhiGjatesia - Konfigurimi.PrefixGjatesia - Konfigurimi.TiketGjatesia;
                return new TcpFileKomunikues(klienti, tcpStream, coreServiceLocator.MerrTransferues(),
                    coreServiceLocator.MerrKomunikues(), tiketa, fajllGjatesia);
            }

            throw new InvalidOperationException();
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
