using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Protokoli.Sherbimet.Abstrakt
{
    /// <summary>
    /// Mundeson transferin nga nje stream ne tjetrin.
    /// Madhesia eshte e paradefinuar, dmth pa framing.
    /// </summary>
    public interface IStreamTransferShkruajtes
    {
        event EventHandler<PaketTransferuarEventArgs> PaketDerguar;

        event EventHandler<PaketTransferuarEventArgs> PaketPranuar;

        Task DergoStreamAsync(int gjatesia, Stream derguesi, Stream pranuesi);

        Task PranoStreamAsync(int gjatesia, Stream derguesi, Stream pranuesi);
    }
}
