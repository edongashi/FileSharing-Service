using System;
using System.IO;
using System.Threading.Tasks;
using FileSharing.Core.Protokoli;
using FileSharing.Klienti.Modeli;

namespace FileSharing.Klienti.Sherbimet.Abstrakt
{
    public interface IServerKomunikues : IDisposable
    {
        Task DergoMesazhAsync(Mesazh mesazhi);

        Task<Mesazh> PranoMesazhAsync();

        Task<Mesazh> KomunikoDyAnshemAsync(Mesazh mesazhi);

        Task<Stream> HapTransferStreamAsync();

        Task<TransferInfo> MerrTransferInfoAsync();

        void MbyllTransferStream();
    }
}