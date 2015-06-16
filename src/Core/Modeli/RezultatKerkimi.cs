using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Core.Modeli
{
    [Serializable]
    public class RezultatKerkimi
    {
        public string Emri { get; set; }

        public LlojiRezultatit LlojiRezultatit { get; set; }

        public FajllInfo[] Fajllat { get; set; }
    }
}
