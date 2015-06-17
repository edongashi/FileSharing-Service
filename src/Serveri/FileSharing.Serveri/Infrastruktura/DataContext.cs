using System.Data.Entity;
using FileSharing.Core.Modeli;

namespace FileSharing.Serveri.Infrastruktura
{
    public class DataContext : DbContext
    {
        public DataContext(string connection)
            : base(connection)
        {
        }

        public DbSet<FajllInfo> Fajllat { get; set; }

        public DbSet<Shfrytezues> Shfrytezuesit { get; set; }
    }
}




