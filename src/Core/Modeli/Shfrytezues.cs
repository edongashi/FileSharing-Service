using System;
using System.ComponentModel.DataAnnotations;

namespace FileSharing.Core.Modeli
{
    [Serializable]
    public class Shfrytezues
    {
        [Key]
        public string Emri { get; set; }

        public string Fjalekalimi { get; set; }
    }
}
