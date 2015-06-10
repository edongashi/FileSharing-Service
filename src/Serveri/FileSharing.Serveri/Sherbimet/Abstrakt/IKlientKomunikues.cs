﻿using System.Threading.Tasks;
using FileSharing.Core.Protokoli;
using Shared.Protokoli;

namespace ServeriCore.Sherbimet.Abstrakt
{
    /// <summary>
    /// Paraqet komunikimin e dyanshem me klient ne kanalin komunikues.
    /// Duhet te mbaje gjendje, dmth lidhjen me klient.
    /// </summary>
    public interface IKlientKomunikues
    {
        Task<Mesazh> PranoAsync();

        Task DergoAsync(Mesazh mesazhi);
    }
}