using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Core
{
    public static class IntegerKonvertuesi
    {
        public static byte[] NeBajta(int numri)
        {
            return BitConverter.GetBytes(numri);
        }

        public static bool ProvoNgaBajtat(byte[] bajtat, out int numri)
        {
            if (bajtat.Length != 4)
            {
                numri = 0;
                return false;
            }

            try
            {
                numri = BitConverter.ToInt32(bajtat, 0);
                return true;
            }
            catch
            {
                numri = 0;
                return false;
            }
        }
    }
}
