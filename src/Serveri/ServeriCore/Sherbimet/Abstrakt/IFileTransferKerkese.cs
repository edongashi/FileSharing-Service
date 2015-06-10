using System.Threading.Tasks;

namespace ServeriCore.Sherbimet.Abstrakt
{
    /// <summary>
    /// Paraqet kerkese per kanalin e transferit. Nuk duhet te mbaje gjendje.
    /// </summary>
    public interface IFileTransferKerkese
    {
        byte[] Kerkesa { get; }

        Task KthePergjigjenAsync(byte[] pergjigja);
    }
}
