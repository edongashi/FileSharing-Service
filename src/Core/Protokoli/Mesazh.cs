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

        public Mesazh(byte header)
            : this(header, new byte[0])
        {
            
        }

        public byte Header { get; set; }

        public byte[] TeDhenat { get; set; }

        public int Gjatesia
        {
            get { return TeDhenat.Length + 1; }
        }
    }
}
