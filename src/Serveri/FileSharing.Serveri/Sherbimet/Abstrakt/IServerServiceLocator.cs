using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Serveri.Sherbimet.Abstrakt
{
    /// <summary>
    /// Ofton sherbime konkrete nga shtresa e serverit.
    /// </summary>
    public interface IServerServiceLocator
    {
        IKlientPranues MerrKlientPranues();

        IFileKomunikuesPranues MerrFilePranues();
    }
}
