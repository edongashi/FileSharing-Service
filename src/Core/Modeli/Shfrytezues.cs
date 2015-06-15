using System;
using System.ComponentModel.DataAnnotations;

namespace FileSharing.Core.Modeli
{
    [Serializable]
    public class Shfrytezues
    {
        public Shfrytezues() { }

        public Shfrytezues(string emri, string fjalekalimi)
        {
            Emri = emri;
            Fjalekalimi = fjalekalimi;
        }

        [Key]
        public string Emri { get; set; }

        public string Fjalekalimi { get; set; }
    }
}
