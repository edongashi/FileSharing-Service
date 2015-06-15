using System;
using System.Globalization;
using System.Threading;
using FileSharing.Core;
using FileSharing.Klienti.UI.ViewModels;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace FileSharing.Klienti.UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly ViewModel viewModel;

        public MainWindow()
        {
            Action<Action> uiInvoker = action => Dispatcher.Invoke(action);
            DataContext = viewModel = new ViewModel(uiInvoker, new WindowDialogShfaqes(this), new FileDialogShfaqes(),
                new DefaultKlientServices(new DefaultCoreServices()));
            InitializeComponent();
            Closing += (s, e) => viewModel.Dispose();
            Loaded += async (s, e) => await viewModel.StartoAsync("Identifikohuni në server");
        }
    }
}