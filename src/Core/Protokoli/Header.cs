namespace FileSharing.Core.Protokoli
{
    public static class Header
    {
        public const byte PaHeader = 0;

        // Klient kerkesat

        public const byte KeepAlive = 1;

        public const byte Identifikim = 2;

        public const byte NderroPassword = 3;

        public const byte Ckycje = 4;

        public const byte MerrFajllat = 5;

        public const byte Search = 6;

        public const byte FileDownload = 7;

        public const byte FileUpload = 8;

        // Server pergjigjet

        public const byte Ok = 9;

        public const byte Gabim = 10;

        public const byte IdentifikimGabim = 11;

        public const byte PermissionGabim = 12;

        public const byte ServerBusy = 13;

        public const byte FileNotFound = 14;

        public const byte FileNePerdorim = 15;

        public const byte BadTicket = 16;

        public const byte ParseGabim = 17;

        public const byte HashFail = 18;
    }
}
