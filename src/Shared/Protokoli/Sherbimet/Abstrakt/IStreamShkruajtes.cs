using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Protokoli.Sherbimet.Abstrakt
{
    public interface IStreamShkruajtes
    {
        Task ShkruajMesazhAsync(Stream streami, Mesazh mesazhi);

        Task<Mesazh> LexoMesazhAsync(Stream streami);
    }
}
