using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Protokoli
{
    public class Mesazh
    {
        public byte Header { get; set; }

        public byte[] TeDhenat { get; set; }
    }
}
