using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using ServeriCore.Model;
using ServeriCore.Sherbimet.Abstrakt;

namespace ServeriCore
{
    public class Server
    {
        private bool startuar;

        public Server(IUserManager repository, IKlientPranues klientPranuesi, IFileKomunikuesPranues komunikuesPranuesi)
        {
            Repository = repository;
            KlientPranuesi = klientPranuesi;
            KomunikuesPranuesi = komunikuesPranuesi;
            KlientetLoguar = new Dictionary<string, IKlientKomunikues>();
            Transferet = new Dictionary<int, TransferTikete>();
        }

        #region Gjendja

        // Mban sesion per shfrytezuesit e loguar.
        public Dictionary<string, IKlientKomunikues> KlientetLoguar { get; set; }

        // Ruan tiketat e transfereve ne nje dictionary.
        // Klienti ben kerkesen ne kanalin e komunikimit dhe i jepet shifra 
        // dhe me pas dorezon keto shifra ne kanalin e transferit.
        public Dictionary<int, TransferTikete> Transferet { get; private set; }

        #endregion

        #region Komponentet

        public IUserManager Repository { get; set; }

        public IKlientPranues KlientPranuesi { get; private set; }

        public IFileKomunikuesPranues KomunikuesPranuesi { get; private set; }

        #endregion

        public void Starto()
        {
            if (startuar)
            {
                return;
            }

            KlientPranuesi.Starto();
            KomunikuesPranuesi.Starto();
            Task.Factory.StartNew(DegjoKomunikim, TaskCreationOptions.LongRunning);
            Task.Factory.StartNew(DegjoTransfer, TaskCreationOptions.LongRunning);
            startuar = true;
        }

        public void Ndalo()
        {
            startuar = false;
            KlientPranuesi.Ndalo();
            KomunikuesPranuesi.Ndalo();
            KlientetLoguar.Clear();
            Transferet.Clear(); 
        }

        private async void DegjoKomunikim()
        {
            while (startuar)
            {
                var klientiKomunikuesi = await KlientPranuesi.PranoKlientAsync();
                Task.Run(() => BisedoMeKlient(klientiKomunikuesi));
            }
        }

        private async void BisedoMeKlient(IKlientKomunikues komunikuesi)
        {
            try
            {
                var kerkesa = await komunikuesi.PranoAsync();
                //if ()
            }
            catch
            {
                return;
            }


        }

        private async void DegjoTransfer()
        {
            while (startuar)
            {
                var kerkesaKomunikuesi = await KomunikuesPranuesi.PranoFileKomunikuesAsync();
            }
        }
    }
}
