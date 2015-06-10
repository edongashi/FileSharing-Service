using System.IO;
using System.Threading.Tasks;
using FileSharing.Core.Protokoli;

namespace Shared.Protokoli.Sherbimet.Abstrakt
{
    /// <summary>
    /// Mundeson komunikimin e dyanshem ne stream
    /// </summary>
    public interface IStreamShkruajtes
    {
        Task ShkruajMesazhAsync(Stream streami, Mesazh mesazhi);

        Task<Mesazh> LexoMesazhAsync(Stream streami);
    }
}
