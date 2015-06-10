using System.Threading.Tasks;

namespace ServeriCore.Sherbimet.Abstrakt
{
    public interface IKlientPranues
    {
        Task<IKlientKomunikues> PranoKlientAsync();
    }
}
