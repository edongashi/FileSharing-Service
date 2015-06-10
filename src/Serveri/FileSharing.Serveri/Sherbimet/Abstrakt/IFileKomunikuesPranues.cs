using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServeriCore.Sherbimet.Abstrakt
{
    public interface IFileKomunikuesPranues
    {
        Task<IFileKomunikues> PranoFileKomunikuesAsync();

        void Starto();

        void Ndalo();
    }
}
