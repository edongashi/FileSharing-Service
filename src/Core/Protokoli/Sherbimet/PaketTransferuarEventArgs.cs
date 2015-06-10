using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Protokoli.Sherbimet
{
    public class PaketTransferuarEventArgs : EventArgs
    {
        public PaketTransferuarEventArgs(int paketatTransferuara, int paketatTotal)
        {
            PaketatTransferuara = paketatTransferuara;
            PaketatTotal = paketatTotal;
        }

        public int PaketatTransferuara { get; private set; }

        public int PaketatTotal { get; private set; }
    }
}
