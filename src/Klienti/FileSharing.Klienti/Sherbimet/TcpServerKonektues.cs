using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using FileSharing.Core;
using FileSharing.Core.Modeli;
using FileSharing.Core.Protokoli;
using FileSharing.Klienti.Sherbimet.Abstrakt;

namespace FileSharing.Klienti.Sherbimet
{
    public class TcpServerKonektues : IServerKonektues
    {
        private readonly SemaphoreSlim loginSemafori = new SemaphoreSlim(1, 1);

        public TcpServerKonektues(IAddressMarres addressMarresi, ICoreServiceLocator coreSherbimet)
        {
            AddressMarresi = addressMarresi;
            CoreSherbimet = coreSherbimet;
        }

        public IAddressMarres AddressMarresi { get; set; }

        public ICoreServiceLocator CoreSherbimet { get; private set; }   

        public async Task<Klient> KonektohuAsync(Shfrytezues shfrytezuesi)
        {
            await loginSemafori.WaitAsync();
            try
            {
                var klienti = new TcpClient();
                await klienti.ConnectAsync(AddressMarresi.ServerAdresa, AddressMarresi.PortiKomunikimit);
                var tcpStream = klienti.GetStream();
                var komunikuesi = CoreSherbimet.MerrKomunikues();
                await komunikuesi.ShkruajMesazhAsync(tcpStream,
                    new Mesazh(Header.Identifikim, XmlSerializuesi<Shfrytezues>.SerializoBajt(shfrytezuesi)));

                var pergjigja = await komunikuesi.LexoMesazhAsync(tcpStream);
                if (pergjigja.Header == Header.Ok)
                {
                    return new Klient(shfrytezuesi.Emri, pergjigja.Teksti, AddressMarresi.ServerAdresa.ToString(),
                        new TcpServerKomunikues(klienti, tcpStream, AddressMarresi, CoreSherbimet), CoreSherbimet);
                }
                else
                {
                    tcpStream.Close();
                    klienti.Close();
                    return null;
                }
            }
            finally
            {
                loginSemafori.Release();
            }
        }
    }
}
