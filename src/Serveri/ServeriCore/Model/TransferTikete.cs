using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServeriCore.Model
{
    /// <summary>
    /// Paraqet nje te dhenat per nje file transfer te kerkuar nga shfrytezuesi.
    /// </summary>
    public class TransferTikete
    {
        public FajllInfo Fajlli { get; set; }

        public KahuTransferit Kahu { get; set; }

        public DateTime KohaKerkeses { get; set; }

        public DateTime DataSkadimit { get; set; }
    }
}
