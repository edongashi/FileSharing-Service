using FileSharing.Core.Modeli;

namespace FileSharing.Klienti.Modeli
{
    public class QueuedFileTransfer
    {
        public int Id { get; set; }

        public KahuTransferit KahuTransferit { get; set; }

        public string Identifier { get; set; }

        public ProgresiTransferit ProgresiTransferit { get; set; }
    }
}
