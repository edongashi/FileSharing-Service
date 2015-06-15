using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSharing.Core.Modeli;
using MahApps.Metro.Controls;

using MahApps.Metro.Controls.Dialogs;

namespace FileSharing.Klienti.UI
{
    class WindowDialogShfaqes : IDialogShfaqes
    {
        private readonly MetroWindow window;

        private readonly MetroDialogSettings dialogSettings = new MetroDialogSettings
        {
            AnimateShow = false
        };

        private readonly LoginDialogSettings loginDialogSettings = new LoginDialogSettings
        {
            UsernameWatermark = "Shfrytëzuesi",
            PasswordWatermark = "Fjalëkalimi",
            AnimateHide = false
        };

        public WindowDialogShfaqes(MetroWindow window)
        {
            this.window = window;
        }

        public async Task<Shfrytezues> KerkoLoginAsync(string mesazhi)
        {
            LoginDialogData rezultati;
            do
            {
                rezultati = await window.ShowLoginAsync("Identifikimi", mesazhi, loginDialogSettings);
            } while (rezultati == null);
            return new Shfrytezues(rezultati.Username, rezultati.Password);
        }

        public Task<ProgressDialogController> ShfaqProgresAsync(string titulli, string mesazhi)
        {
            return window.ShowProgressAsync(titulli, mesazhi, settings: dialogSettings);
        }

        public Task ShfaqMesazhAsync(string titulli, string mesazhi)
        {
            return window.ShowMessageAsync(titulli, mesazhi, MessageDialogStyle.Affirmative, dialogSettings);
        }
    }
}
