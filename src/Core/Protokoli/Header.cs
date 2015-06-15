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

        public const byte FileDelete = 9;

        public const byte BejePublik = 10;

        public const byte BejePrivat = 11;

        // Server pergjigjet

        public const byte Ok = 20;

        public const byte Gabim = 21;

        public const byte IdentifikimGabim = 22;

        public const byte InvalidUserOsePass = 23;

        public const byte PermissionGabim = 24;

        public const byte ServerBusy = 25;

        public const byte FileNotFound = 26;

        public const byte FileNePerdorim = 27;

        public const byte BadTicket = 28;

        public const byte ParseGabim = 29;

        public const byte HashFail = 30;
    }
}
