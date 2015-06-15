using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Klienti.Sherbimet.Abstrakt
{
    public interface IAddressMarres
    {
        IPAddress ServerAdresa { get; }

        int PortiKomunikimit { get; }

        int PortiTransferit { get; }
    }
}
