using System;
using System.ComponentModel.DataAnnotations;

namespace FileSharing.Core.Modeli
{
    [Serializable]
    public class FajllInfo
    {
        [Key]
        public int Id { get; set; }

        public string Emri { get; set; }

        public int Madhesia { get; set; }

        public string Pronari { get; set; }

        public Dukshmeria Dukshmeria { get; set; }

        public DateTime DataShtimit { get; set; }
    }
}
