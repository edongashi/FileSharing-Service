namespace FileSharing.Core.Protokoli
{
    public static class Header
    {
        public const byte PaHeader = 0;

        // Klient kerkesat

        public const byte KeepAlive = 1;

        public const byte Identifikim = 2;

        public const byte KrijoUser = 3;

        public const byte NderroPassword = 4;

        public const byte Ckycje = 5;

        public const byte MerrFajllat = 6;

        public const byte Search = 7;

        public const byte FileDownload = 8;

        public const byte FileUpload = 9;

        public const byte FileDelete = 10;

        public const byte BejePublik = 11;

        public const byte BejePrivat = 12;

        public const byte MerrLink = 13;

        // Server pergjigjet

        public const byte Ok = 20;

        public const byte Gabim = 21;

        public const byte IdentifikimGabim = 22;

        public const byte InvalidUserOsePass = 23;

        public const byte UserLoguarGabim = 24;

        public const byte PermissionGabim = 25;

        public const byte ServerBusy = 26;

        public const byte FileNotFound = 27;

        public const byte FileNePerdorim = 28;

        public const byte BadTicket = 29;

        public const byte ParseGabim = 30;

        public const byte HashFail = 31;
    }
}
