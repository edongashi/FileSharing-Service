using System;
using System.Collections.Generic;
using System.Data.Entity;
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
            if (string.IsNullOrEmpty(useri) || string.IsNullOrEmpty(passwordi))
            {
                return false;
            }

            var hash = MerrHash(passwordi);
            using (var db = new DataContext(connection))
            {
                return db.Shfrytezuesit.Any(sh => sh.Emri == useri && sh.Fjalekalimi == hash);
            }
        }

        public bool KrijoUser(string useri, string passwordi)
        {
            if (string.IsNullOrEmpty(useri) || string.IsNullOrEmpty(passwordi))
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

        public FajllInfo MerrFajllInfo(int id)
        {
            using (var db = new DataContext(connection))
            {
                return db.Fajllat.Find(id);
            }
        }

        public bool UpdateFajll(FajllInfo fajlli)
        {
            using (var db = new DataContext(connection))
            {
                try
                {
                    db.Entry(fajlli).State = EntityState.Modified;
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public bool DeleteFajll(FajllInfo fajlli)
        {
            using (var db = new DataContext(connection))
            {
                try
                {
                    db.Entry(fajlli).State = EntityState.Deleted;
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public FajllInfo[] MerrFajllatUserit(string useri)
        {
            using (var db = new DataContext(connection))
            {
                return db.Fajllat.Where(fajlli => fajlli.Pronari == useri).ToArray();
            }
        }

        public FajllInfo[] MerrFajllatPublikUserit(string useri)
        {
            using (var db = new DataContext(connection))
            {
                return
                    db.Fajllat.Where(fajlli => fajlli.Pronari == useri && fajlli.Dukshmeria == Dukshmeria.Publike)
                        .ToArray();
            }
        }

        public RezultatKerkimi[] Kerko(string pronari, string termi)
        {
            using (var db = new DataContext(connection))
            {
                var fajllatRezultati = (
                    from fajll in db.Fajllat
                    where (fajll.Pronari == pronari
                           || fajll.Dukshmeria == Dukshmeria.Publike)
                          && fajll.Emri.StartsWith(termi)
                    select fajll
                    ).ToArray();

                var shfrytezuesit =
                    from user in db.Shfrytezuesit
                    where user.Emri.StartsWith(termi) && user.Emri != pronari
                    join fajll in db.Fajllat.Where(f => f.Dukshmeria == Dukshmeria.Publike)
                        on user.Emri equals fajll.Pronari into fajllat
                    select new { user.Emri, Fajllat = fajllat };

                var shfrytezuesitRezultati = shfrytezuesit.ToArray().Select(val =>
                    new RezultatKerkimi
                    {
                        Emri = val.Emri,
                        Fajllat = val.Fajllat.ToArray(),
                        LlojiRezultatit = LlojiRezultatit.Shfrytezues
                    });

                if (fajllatRezultati.Length > 0)
                {
                    var teksti = fajllatRezultati.Length > 1 ? " Fajlla" : " Fajll";
                    return new[]
                    {
                        new RezultatKerkimi
                        {
                            Emri = fajllatRezultati.Length + teksti,
                            LlojiRezultatit = LlojiRezultatit.Fajll,
                            Fajllat = fajllatRezultati
                        }
                    }.Concat(shfrytezuesitRezultati).ToArray();
                }

                return shfrytezuesitRezultati.ToArray();
            }
        }

        private string MerrHash(string plain)
        {
            var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(plain));
            return BitConverter.ToString(hash);
        }
    }
}