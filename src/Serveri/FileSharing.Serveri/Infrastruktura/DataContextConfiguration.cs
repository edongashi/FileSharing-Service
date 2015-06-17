using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.SqlServerCompact;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileSharing.Serveri.Infrastruktura
{
    public class DataContextConfiguration : DbConfiguration
    {
        public DataContextConfiguration()
        {
            SetProviderServices(
                SqlCeProviderServices.ProviderInvariantName,
                SqlCeProviderServices.Instance);
        }
    }
}



