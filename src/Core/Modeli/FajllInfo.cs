using System;

namespace FileSharing.Core.Modeli
{
    [Serializable]
    public class FajllInfo
    {
        public string Emri { get; set; }

        public int Madhesia { get; set; }

        public string Pronari { get; set; }

        public Dukshmeria Dukshmeria { get; set; }

        public DateTime DataShtimit { get; set; }
    }
}
