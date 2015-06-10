using System.Collections.Generic;
using ServeriCore.Model;

namespace ServeriCore.Sherbimet.Abstrakt
{
    /// <summary>
    /// Mundeson lidhje ne baze te dhenave per te lexuar dhe menaxhuar shfrytezuesit.
    /// </summary>
    public interface IUserManager
    {
        bool TestoLogin(string useri, string passwordi);

        bool ProvoKrijoUser(string useri, string passwordi);

        bool ProvoNderroPassword(string useri, string passVjeter, string passRi);

        IEnumerable<FajllInfo> MerrFajllatUserit(string useri);

        bool ShtoFajllPerUser(string user, FajllInfo fajlli);

        IEnumerable<FajllInfo> MerrFajllatPublikUserit(string useri);
    }
}
