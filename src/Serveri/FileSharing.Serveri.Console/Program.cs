using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FileSharing.Core;
using FileSharing.Serveri.Infrastruktura;

namespace FileSharing.Serveri.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var pathResolver = new StandardPaths();
            var repository = new SqlCeRepository(pathResolver.GetFileInRootPath("databaza.db"));
            var serverSherbimet = new DefaultServerServices(new DefaultCoreServices());
            var serveri = new Server("Edon-PC", repository, pathResolver, serverSherbimet, 5);
            serveri.Starto();
            System.Console.ReadLine();
        }
    }
}
