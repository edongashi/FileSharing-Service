using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSharing.Core.Modeli;

namespace FileSharing.Klienti
{
    public class FajllInfoEventArgs : EventArgs
    {
        public FajllInfoEventArgs(FajllInfo fajllInfo)
        {
            FajllInfo = fajllInfo;
        }

        public FajllInfo FajllInfo { get; private set; }
    }
}
