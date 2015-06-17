using System.ServiceProcess;
using FileSharing.Core;
using FileSharing.Serveri.Infrastruktura;

namespace FileSharing.Serveri.Service
{
    public partial class FileSharingSherbimi : ServiceBase
    {
        private Server serveri;

        public FileSharingSherbimi()
        {
            InitializeComponent();
            var pathResolver = new ServicePaths();
            var repository = new SqlCeRepository(pathResolver.GetFileInRootPath("databaza.db"));
            var serverSherbimet = new DefaultServerServices(new DefaultCoreServices());
            serveri = new Server("Edon-PC", "123456", repository, pathResolver, serverSherbimet, 5);
        }

        protected override void OnStart(string[] args)
        {
            serveri.Starto();
        }

        protected override void OnStop()
        {
            serveri.Ndalo();
        }
    }
}
