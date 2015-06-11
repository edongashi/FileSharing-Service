using FileSharing.Core.Protokoli.Sherbimet;
using FileSharing.Core.Protokoli.Sherbimet.Abstrakt;

namespace FileSharing.Core
{
    public class DefaultCoreServices : ICoreServiceLocator
    {
        public IStreamShkruajtes MerrKomunikues()
        {
            return new StreamShkruajtes();
        }

        public IStreamTransferShkruajtes MerrTransferues()
        {
            return new StreamTransferShkruajtes();
        }
    }
}
