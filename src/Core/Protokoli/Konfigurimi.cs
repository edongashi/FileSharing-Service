namespace FileSharing.Core.Protokoli
{
    public static class Konfigurimi
    {
        public const int PrefixGjatesia = sizeof(int);

        public const int TiketGjatesia = sizeof(int);

        public const int PaketMadhesia = 1024;

        public const bool TestoHash = true;

        public const string HashAlgoritmi = "MD5";
    }
}