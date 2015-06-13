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

        FajllInfo MerrFajllInfo(int id);

        IEnumerable<FajllInfo> MerrFajllatUserit(string useri);

        bool ShtoFajll(FajllInfo fajlli);

        IEnumerable<FajllInfo> MerrFajllatPublikUserit(string useri);
    }
}
