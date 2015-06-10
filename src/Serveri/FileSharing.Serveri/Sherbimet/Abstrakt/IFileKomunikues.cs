using System.Threading.Tasks;
using FileSharing.Core.Protokoli;
using Shared.Protokoli;

namespace ServeriCore.Sherbimet.Abstrakt
{
    /// <summary>
    /// Paraqet kerkese per kanalin e transferit. Nuk duhet te mbaje gjendje.
    /// </summary>
    public interface IFileKomunikues
    {
        int TiketaId { get; }

        Task<byte[]> MerrTeDhenat();

        Task KthePergjigjeAsync(Mesazh pergjigja);
    }
}
