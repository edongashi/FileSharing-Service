using System.Threading.Tasks;

namespace FileSharing.Serveri.Sherbimet.Abstrakt
{
    public interface IKlientPranues
    {
        Task<IKlientKomunikues> PranoKlientAsync();

        void Starto();

        void Ndalo();
    }
}
