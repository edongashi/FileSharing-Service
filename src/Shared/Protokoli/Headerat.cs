using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Protokoli
{
    public static class Headerat
    {
        public const byte KeepAlive = 0;

        public const byte Identifikim = 1;

        public const byte Ckycje = 2;

        public const byte MerrFajllat = 3;

        public const byte Search = 4;

        public const byte FileDownload = 5;

        public const byte FileUpload = 6;

        public const byte Ok = 7;

        public const byte ServerBusy = 8;

        public const byte LoginGabim = 9;

        public const byte FileNotFound = 10;

        public const byte BadTicket = 11;

        public const byte ParseGabim = 12;

        public const byte HashFail = 13;
    }
}
