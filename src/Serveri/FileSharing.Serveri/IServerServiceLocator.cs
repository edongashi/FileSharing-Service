using FileSharing.Serveri.Sherbimet.Abstrakt;

namespace FileSharing.Serveri
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
