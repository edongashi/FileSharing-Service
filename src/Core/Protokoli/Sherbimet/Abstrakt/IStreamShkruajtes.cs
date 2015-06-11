using System.IO;
using System.Threading.Tasks;

namespace FileSharing.Core.Protokoli.Sherbimet.Abstrakt
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
