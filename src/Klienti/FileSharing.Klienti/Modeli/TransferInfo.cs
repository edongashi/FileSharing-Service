using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Klienti.Modeli
{
    public class TransferInfo
    {
        public TransferInfo(byte header, int fajlliGjatesia)
        {
            Header = header;
            FajlliGjatesia = fajlliGjatesia;
        }

        public byte Header { get; private set; }

        public int FajlliGjatesia { get; private set; }
    }
}
