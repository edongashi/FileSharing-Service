using FileSharing.Core.Protokoli.Sherbimet.Abstrakt;

namespace FileSharing.Core
{
    public interface ICoreServiceLocator
    {
        IStreamShkruajtes MerrKomunikues();

        IStreamTransferShkruajtes MerrTransferues();
    }
}
