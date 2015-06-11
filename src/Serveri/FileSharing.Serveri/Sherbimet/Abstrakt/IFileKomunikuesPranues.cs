using System.Threading.Tasks;

namespace FileSharing.Serveri.Sherbimet.Abstrakt
{
    public interface IFileKomunikuesPranues
    {
        Task<IFileKomunikues> PranoFileKomunikuesAsync();

        void Starto();

        void Ndalo();
    }
}
