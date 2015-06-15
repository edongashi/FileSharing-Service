using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Klienti.UI
{
    public interface IFileDialogShfaqes
    {
        string MerrOpenPath();

        string MerrSavePath(string emri);
    }
}
