using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSharing.Core;
using FileSharing.Klienti.Sherbimet;
using FileSharing.Klienti.Sherbimet.Abstrakt;

namespace FileSharing.Klienti
{
    public class DefaultKlientServices : IKlientServiceLocator
    {
        public DefaultKlientServices(ICoreServiceLocator coreSherbimet)
        {
            CoreSherbimet = coreSherbimet;
        }

        public ICoreServiceLocator CoreSherbimet { get; private set; }

        public IServerKonektues MerrServerKonektues()
        {
            return new TcpServerKonektues(new ConfigAddressMarres(), CoreSherbimet);
        }
    }
}
