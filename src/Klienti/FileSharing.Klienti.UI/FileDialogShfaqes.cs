using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace FileSharing.Klienti.UI
{
    public class FileDialogShfaqes : IFileDialogShfaqes
    {
        public string MerrOpenPath()
        {
            var openFileDialog = new OpenFileDialog();

            return openFileDialog.ShowDialog() == true ? openFileDialog.FileName : null;
        }

        public string MerrSavePath(string emri)
        {
            var saveFileDialog = new SaveFileDialog()
            {
                FileName = emri
            };

            return saveFileDialog.ShowDialog() == true ? saveFileDialog.FileName : null;
        }
    }
}
