using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using FileSharing.Core;
using FileSharing.Klienti.Modeli;
using FileSharing.Klienti.Sherbimet.Abstrakt;

namespace FileSharing.Klienti.Sherbimet
{
    public class ConfigAddressMarres : IAddressMarres
    {
        public ConfigAddressMarres()
        {
            var serverInfo = XmlSerializuesi<ServerInfo>.DeserializoBajt(File.ReadAllBytes("server_info.xml"));
            ServerAdresa = IPAddress.Parse(serverInfo.Adresa);
            PortiKomunikimit = serverInfo.PortiKomunikimit;
            PortiTransferit = serverInfo.PortiTransferit;
        }

        public IPAddress ServerAdresa { get; private set; }

        public int PortiKomunikimit { get; private set; }

        public int PortiTransferit { get; private set; }
    }
}
