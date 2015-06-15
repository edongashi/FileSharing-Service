using System.Text;

namespace FileSharing.Core.Protokoli
{
    /// <summary>
    /// Paraqet mesazhet qe shkembehen ndermjet klientit dhe serverit.
    /// </summary>
    public class Mesazh
    {
        public Mesazh(byte header, byte[] teDhenat)
        {
            Header = header;
            TeDhenat = teDhenat;
        }

        public Mesazh(byte header, string permbajtja)
            : this(header, Encoding.UTF8.GetBytes(permbajtja))
        {
        }

        public Mesazh(byte header)
            : this(header, new byte[0])
        {
            
        }

        public byte Header { get; set; }

        public byte[] TeDhenat { get; set; }

        public string Teksti
        {
            get { return Encoding.UTF8.GetString(TeDhenat); }
        }

        public int Gjatesia
        {
            get
            {
                if (Header == Protokoli.Header.PaHeader)
                {
                    return TeDhenat.Length;
                }
                else
                {
                    return TeDhenat.Length + 1;
                }
            }
        }
    }
}