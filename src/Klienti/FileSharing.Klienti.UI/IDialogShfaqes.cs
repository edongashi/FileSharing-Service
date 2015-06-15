using System.Threading.Tasks;
using FileSharing.Core.Modeli;
using MahApps.Metro.Controls.Dialogs;

namespace FileSharing.Klienti.UI
{
    public interface IDialogShfaqes
    {
        Task<Shfrytezues> KerkoLoginAsync(string mesazhi);

        Task<ProgressDialogController> ShfaqProgresAsync(string titulli, string mesazhi);

        Task ShfaqMesazhAsync(string titulli, string mesazhi);
    }
}
