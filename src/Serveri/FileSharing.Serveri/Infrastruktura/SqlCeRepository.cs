using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FileSharing.Core.Modeli;
using FileSharing.Serveri.Infrastruktura.Abstrakt;
using FileSharing.Serveri.Sherbimet.Abstrakt;

namespace FileSharing.Serveri.Infrastruktura
{
    public class SqlCeRepository : IRepository
    {
        private readonly string connection;

        private readonly SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

        public SqlCeRepository(string dbFile)
        {
            connection = "Data Source=" + dbFile + ";";
        }

        public bool TestoLogin(string useri, string passwordi)
        {
            var hash = MerrHash(passwordi);
            using (var db = new DataContext(connection))
            {
                return db.Shfrytezuesit.Any(sh => sh.Emri == useri && sh.Fjalekalimi == hash);
            }
        }

        public bool KrijoUser(string useri, string passwordi)
        {
            if (string.IsNullOrEmpty(passwordi))
            {
                return false;
            }


            using (var db = new DataContext(connection))
            {
                try
                {
                    var hash = MerrHash(passwordi);
                    db.Shfrytezuesit.Add(new Shfrytezues
                    {
                        Emri = useri,
                        Fjalekalimi = hash
                    });

                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool NderroPassword(string useri, string passVjeter, string passRi)
        {
            if (string.IsNullOrEmpty(passRi))
            {
                return false;
            }

            using (var db = new DataContext(connection))
            {
                try
                {
                    var hash = MerrHash(passVjeter);
                    var hashRi = MerrHash(passRi);
                    var userEntity = db.Shfrytezuesit.Find(useri);
                    if (userEntity != null && userEntity.Fjalekalimi == hash)
                    {
                        userEntity.Fjalekalimi = hashRi;
                        db.SaveChanges();
                        return true;
                    }

                    return false;
                }
                catch
                {
                    return false;
                }
            }
        }

        public FajllInfo MerrFajllInfo(int id)
        {
            using (var db = new DataContext(connection))
            {
                return db.Fajllat.Find(id);
            }
        }

        public IEnumerable<FajllInfo> MerrFajllatUserit(string useri)
        {
            using (var db = new DataContext(connection))
            {
                return db.Fajllat.Where(fajlli => fajlli.Pronari == useri);
            }
        }

        public bool ShtoFajll(FajllInfo fajlli)
        {
            using (var db = new DataContext(connection))
            {
                try
                {
                    db.Fajllat.Add(fajlli);
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public IEnumerable<FajllInfo> MerrFajllatPublikUserit(string useri)
        {
            using (var db = new DataContext(connection))
            {
                return db.Fajllat.Where(fajlli => fajlli.Pronari == useri && fajlli.Dukshmeria == Dukshmeria.Publike);
            }
        }

        private string MerrHash(string plain)
        {
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(plain));
            return BitConverter.ToString(hash);
        }
    }
}
