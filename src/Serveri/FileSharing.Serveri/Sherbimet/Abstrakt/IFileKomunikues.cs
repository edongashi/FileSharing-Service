using System;
using System.IO;
using System.Threading.Tasks;
using FileSharing.Core.Protokoli;

namespace FileSharing.Serveri.Sherbimet.Abstrakt
{
    /// <summary>
    /// Paraqet kerkese per kanalin e transferit. Nuk duhet te mbaje gjendje.
    /// </summary>
    public interface IFileKomunikues : IDisposable
    {
        int TiketaId { get; }

        Task PranoFajllAsync(Stream pranuesi);

        Task DergoFajllAsync(byte header, Stream derguesi, int gjatesia);

        Task KthePergjigjeAsync(Mesazh pergjigja);
    }
}
