using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Protokoli.Sherbimet.Abstrakt
{
    public interface IStreamTransferShkruajtes
    {
        Task DergoStreamAsync(byte header, Stream derguesi, Stream pranuesi);

        Task PranoStreamAsync(byte header, Stream derguesi, Stream pranuesi);
    }
}
