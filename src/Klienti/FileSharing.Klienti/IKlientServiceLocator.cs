using FileSharing.Klienti.Sherbimet.Abstrakt;

namespace FileSharing.Klienti
{
    public interface IKlientServiceLocator
    {
        IServerKonektues MerrServerKonektues();
    }
}
