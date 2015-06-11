using System;
using System.Threading.Tasks;
using FileSharing.Core.Protokoli;

namespace FileSharing.Serveri.Sherbimet.Abstrakt
{
    /// <summary>
    /// Paraqet komunikimin e dyanshem me klient ne kanalin komunikues.
    /// Duhet te mbaje gjendje, dmth lidhjen me klient.
    /// </summary>
    public interface IKlientKomunikues : IDisposable
    {
        Task<Mesazh> PranoAsync();

        Task DergoAsync(Mesazh mesazhi);
    }
}
