using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using FileSharing.Klienti.Sherbimet.Abstrakt;
using MahApps.Metro;
using FileSharing.Core.Modeli;
using System.Windows.Input;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace FileSharing.Klienti.UI.ViewModels
{
    public class ViewModel : INotifyPropertyChanged, IDisposable
    {
        private readonly Action<Action> uiInvoker;
        private readonly IDialogShfaqes dialogShfaqesi;
        private readonly IFileDialogShfaqes fileDialogShfaqesi;

        private readonly IServerKonektues konektuesi;

        private Klient klienti;

        private ObservableCollection<FajllInfo> fajllat;

        private FajllInfo fajllInfoSelektuar;
        private bool kaPronesiMbiFajllin;

        private int listaSelektuar;

        public ViewModel(Action<Action> uiInvoker, IDialogShfaqes dialogShfaqesi, IFileDialogShfaqes fileDialogShfaqesi, IKlientServiceLocator klientSherbimet)
        {
            this.uiInvoker = uiInvoker;
            this.dialogShfaqesi = dialogShfaqesi;
            this.fileDialogShfaqesi = fileDialogShfaqesi;
            konektuesi = klientSherbimet.MerrServerKonektues();
            ShtoFajllCommand = new DelegateCommand(arg =>
            {
                var path = fileDialogShfaqesi.MerrOpenPath();
                if (!string.IsNullOrEmpty(path))
                {
                    klienti.StartoUploadFile(path);
                }
            });

            ShkarkoFajllCommand = new DelegateCommand(arg =>
            {
                var fajlli = fajllInfoSelektuar;
                var path = fileDialogShfaqesi.MerrSavePath(fajllInfoSelektuar.Emri);
                if (!string.IsNullOrEmpty(path))
                {
                    klienti.StartoDownloadFile(fajlli, path);
                }
            }, arg => fajllInfoSelektuar != null);

            MerrLinkCommand = new DelegateCommand(arg =>
            {

            }, arg => fajllInfoSelektuar != null /* TODO */);

            FshijFajllCommand = new DelegateCommand(async arg =>
            {
                var fajlli = fajllInfoSelektuar;
                var fshire = await klienti.FshijFajllinAsync(fajlli);
                if (fshire)
                {
                    Fajllat.Remove(fajlli);
                }
                else
                {
                    await dialogShfaqesi.ShfaqMesazhAsync("Gabim", "Nuk u krye fshirja e fajllit.");
                }
            }, arg => kaPronesiMbiFajllin);

            BejePublikCommand = new DelegateCommand(async arg =>
            {
                var fajlli = fajllInfoSelektuar;
                var kryer = await klienti.BejPublikAsync(fajlli);
                if (kryer)
                {
                    fajlli.RaisePropetyChanged("EshtePublik");
                    OnPropertyChanged("BejPublikDukshmeria");
                    OnPropertyChanged("BejPrivatDukshmeria");
                }
                else
                {
                    await dialogShfaqesi.ShfaqMesazhAsync("Gabim", "Nuk u realizua kerkesa.");
                }
            }, arg => kaPronesiMbiFajllin);

            BejePrivatCommand = new DelegateCommand(async arg =>
            {
                var fajlli = fajllInfoSelektuar;
                var kryer = await klienti.BejPrivatAsync(fajlli);
                if (kryer)
                {
                    fajlli.RaisePropetyChanged("EshtePublik");
                    OnPropertyChanged("BejPublikDukshmeria");
                    OnPropertyChanged("BejPrivatDukshmeria");
                }
                else
                {
                    await dialogShfaqesi.ShfaqMesazhAsync("Gabim", "Nuk u realizua kerkesa.");
                }
            }, arg => kaPronesiMbiFajllin);

            PastroTransferetCommand = new DelegateCommand(arg => klienti.PastroTransferet());
        }

        public DelegateCommand ShtoFajllCommand { get; private set; }

        public DelegateCommand ShkarkoFajllCommand { get; private set; }

        public DelegateCommand MerrLinkCommand { get; private set; }

        public DelegateCommand FshijFajllCommand { get; private set; }

        public DelegateCommand BejePublikCommand { get; private set; }

        public DelegateCommand BejePrivatCommand { get; private set; }

        public DelegateCommand PastroTransferetCommand { get; private set; }

        public ObservableCollection<FajllInfo> Fajllat
        {
            get { return fajllat; }
            set
            {
                fajllat = value;
                OnPropertyChanged("Fajllat");
            }
        }

        public FajllInfo FajllInfoSelektuar
        {
            get { return fajllInfoSelektuar; }
            set
            {
                fajllInfoSelektuar = value;
                kaPronesiMbiFajllin = value != null &&
                    string.Equals(value.Pronari, klienti.Shfrytezuesi, StringComparison.OrdinalIgnoreCase);
                OnPropertyChanged("FajllinfoSelektuar");
                RifreskoKomandat();
            }
        }

        public bool BejPublikDukshmeria
        {
            get
            {
                return fajllInfoSelektuar == null || (kaPronesiMbiFajllin &&
                    fajllInfoSelektuar.Dukshmeria == Dukshmeria.Private);
            }
        }

        public bool BejPrivatDukshmeria
        {
            get
            {
                return kaPronesiMbiFajllin &&
                    fajllInfoSelektuar.Dukshmeria == Dukshmeria.Publike;
            }
        }

        public bool KaPronesiMbiFajllin
        {
            get { return kaPronesiMbiFajllin; }
        }

        public int ListaSelektuar
        {
            get { return listaSelektuar; }
            set
            {
                listaSelektuar = value;
                OnPropertyChanged("ListaSelektuar");
            }
        }

        public Klient Klienti
        {
            get { return klienti; }
            private set
            {
                klienti = value;
                OnPropertyChanged("Klienti");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public async Task StartoAsync(string mesazhiInicial)
        {
            var konektuar = false;
            var mesazhi = mesazhiInicial;
            while (!konektuar)
            {
                var shfrytezuesi = await dialogShfaqesi.KerkoLoginAsync(mesazhi);
                if (shfrytezuesi == null)
                {
                    continue;
                }

                var progresKontrolleri = await dialogShfaqesi.ShfaqProgresAsync("Duke inicializuar", "");
                try
                {
                    var rezultati = await konektuesi.KonektohuAsync(shfrytezuesi);
                    if (rezultati == null)
                    {
                        mesazhi = "Shfrytëzuesi ose fjalëkalimi i gabuar";
                    }
                    else
                    {
                        Klienti = rezultati;
                        rezultati.FajllShtuar += (s, e) => uiInvoker.Invoke(() => Fajllat.Add(e.FajllInfo));
                        Fajllat = new ObservableCollection<FajllInfo>(await Klienti.MerrFajllatAsync());
                        konektuar = true;
                    }
                }
                catch
                {
                    mesazhi = "Nuk u mundesua lidhja ne server, provoni perseri";
                    konektuar = false;
                }

                await progresKontrolleri.CloseAsync();
            }
        }

        public void Dispose()
        {
            if (klienti != null)
            {
                klienti.Dispose();
            }
        }

        private void RifreskoKomandat()
        {
            OnPropertyChanged("KaPronesiMbiFajllin");
            OnPropertyChanged("BejPublikDukshmeria");
            OnPropertyChanged("BejPrivatDukshmeria");
            MerrLinkCommand.RaiseCanExecuteChanged();
            ShkarkoFajllCommand.RaiseCanExecuteChanged();
            BejePublikCommand.RaiseCanExecuteChanged();
            BejePrivatCommand.RaiseCanExecuteChanged();
            FshijFajllCommand.RaiseCanExecuteChanged();
        }

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
