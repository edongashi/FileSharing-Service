using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSharing.Core.Modeli;

namespace FileSharing.Klienti.Sherbimet.Abstrakt
{
    public interface IServerKonektues
    {
        Task<Klient> KonektohuAsync(Shfrytezues shfrytezuesi);
    }
}
