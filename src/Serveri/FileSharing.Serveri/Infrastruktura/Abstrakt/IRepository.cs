using System.Collections.Generic;
using FileSharing.Core.Modeli;

namespace FileSharing.Serveri.Infrastruktura.Abstrakt
{
    /// <summary>
    /// Mundeson lidhje ne baze te dhenave per te lexuar dhe menaxhuar shfrytezuesit.
    /// </summary>
    public interface IRepository
    {
        bool TestoLogin(string useri, string passwordi);

        bool KrijoUser(string useri, string passwordi);

        bool NderroPassword(string useri, string passVjeter, string passRi);

        bool ShtoFajll(FajllInfo fajlli);

        FajllInfo MerrFajllInfo(int id);

        bool UpdateFajll(FajllInfo fajlli);

        bool DeleteFajll(FajllInfo fajlli);

        FajllInfo[] MerrFajllatUserit(string useri);

        FajllInfo[] MerrFajllatPublikUserit(string useri);
    }
}
