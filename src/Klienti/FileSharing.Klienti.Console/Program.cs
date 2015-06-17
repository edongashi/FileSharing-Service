using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using FileSharing.Core;
using FileSharing.Core.Modeli;
using FileSharing.Core.Protokoli;
using FileSharing.Core.Protokoli.Sherbimet;

namespace FileSharing.Klienti.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            System.Console.WriteLine("=== Command line menaxhimi ===");
            System.Console.Write("Shkruani IP dhe portin e serverit:\n> ");
            var shkruajtesi = new StreamShkruajtes();
            while (true)
            {
                IPEndPoint ipEndPoint;
                try
                {
                    var input = System.Console.ReadLine();
                    if (input == "/dalja")
                    {
                        break;
                    }

                    ipEndPoint = MerrIpEndPoint(input);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    System.Console.Write("> ");
                    continue;
                }

                try
                {

                KonektohuPerseri:
                    var klienti = new TcpClient();
                    klienti.Connect(ipEndPoint);
                    var streami = klienti.GetStream();
                    System.Console.Write("U konektua me sukses, jepni komanden:\n> ");

                LexoPerseri:
                    // ReSharper disable once PossibleNullReferenceException
                    var kerkesa = System.Console.ReadLine().Trim();
                    if (kerkesa == "/dalja")
                    {
                        streami.Close();
                        klienti.Close();
                        break;
                    }

                    if (kerkesa == "/ip")
                    {
                        streami.Close();
                        klienti.Close();
                        System.Console.Write("Shkruani IP dhe portin e serverit:\n> ");
                        continue;
                    }

                    // Kerkesat
                    var parametrat = kerkesa.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (parametrat.Length == 0)
                    {
                        System.Console.Write("> ");
                        goto LexoPerseri;
                    }

                    kerkesa = parametrat[0].ToLower();

                    switch (kerkesa)
                    {
                        case "krijo_user":
                            if (parametrat.Length != 3)
                            {
                                System.Console.Write("Gabim ne format, shkruaj perseri:\n> ");
                                goto LexoPerseri;
                            }

                            var username = parametrat[1];
                            var password = parametrat[2];
                            var objekti = XmlSerializuesi<Shfrytezues>.SerializoBajt(new Shfrytezues(username, password));

                            var mesazhi = new Mesazh(Header.KrijoUser, objekti);
                            shkruajtesi.ShkruajMesazhAsync(streami, mesazhi).Wait();
                            var pergjigja = shkruajtesi.LexoMesazhAsync(streami).Result;
                            if (pergjigja.Header == Header.Ok)
                            {
                                System.Console.Write("Shfrytezuesi u krijua me sukses.\n> ");
                            }
                            else
                            {
                                System.Console.Write("Gabim. Kerkesa nuk eshte realizuar.\n> ");
                            }

                            klienti.Close();
                            streami.Close(250);
                            goto KonektohuPerseri;

                        // Rastet tjera qe do te shtohen

                        default:
                            System.Console.Write("Kerkese e panjohur, shtyp perseri:\n> ");
                            goto LexoPerseri;
                            break;
                    }
                }
                catch
                {
                    System.Console.Write("Nuk u arrit lidhja, provoni perseri:\n> ");
                    continue;
                }
            }
        }

        private static IPEndPoint MerrIpEndPoint(string endPoint)
        {
            string[] ep = endPoint.Split(':');
            if (ep.Length != 2)
            {
                throw new FormatException("Format jo valid.");
            }

            IPAddress ip;
            if (!IPAddress.TryParse(ep[0], out ip))
            {
                throw new FormatException("IP Adresa jo valide.");
            }

            int port;
            if (!int.TryParse(ep[1], out port))
            {
                throw new FormatException("Porti jo valid.");
            }

            return new IPEndPoint(ip, port);
        }
    }
}
