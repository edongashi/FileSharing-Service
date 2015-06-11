using System.Threading.Tasks;
using FileSharing.Core.Protokoli;

namespace FileSharing.Serveri.Sherbimet.Abstrakt
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
