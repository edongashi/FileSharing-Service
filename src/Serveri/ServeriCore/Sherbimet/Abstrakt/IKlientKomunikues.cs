using System.Threading.Tasks;

namespace ServeriCore.Sherbimet.Abstrakt
{
    /// <summary>
    /// Paraqet komunikimin e dyanshem me klient ne kanalin komunikues.
    /// Duhet te mbaje gjendje, dmth lidhjen me klient.
    /// </summary>
    public interface IKlientKomunikues
    {
        Task<byte[]> PranoAsync();

        Task DergoAsync(byte[] mesazhi);
    }
}
