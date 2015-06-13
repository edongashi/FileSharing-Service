using System.Net;
using System.Net.Sockets;
using FileSharing.Core;
using FileSharing.Serveri.Sherbimet;
using FileSharing.Serveri.Sherbimet.Abstrakt;

namespace FileSharing.Serveri
{
    public class DefaultServerServices : IServerServiceLocator
    {
        private readonly ICoreServiceLocator coreServiceLocator;

        public DefaultServerServices(ICoreServiceLocator coreServiceLocator)
        {
            this.coreServiceLocator = coreServiceLocator;
        }

        public IKlientPranues MerrKlientPranues()
        {
            return new TcpKlientPranues(new TcpListener(IPAddress.Any, 7000), coreServiceLocator);
        }

        public IFileKomunikuesPranues MerrFilePranues()
        {
            return new TcpFileKomunikuesPranues(new TcpListener(IPAddress.Any, 7001), coreServiceLocator);
        }
    }
}
