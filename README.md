# FileSharing-Service

## INFO
- Projektet referencojne .NET 4.5.1
- Projektet kane varshmeri te jashtme, ato merren permes NuGet (automatikisht ne VS te ri)

- Klienti GUI aplikacioni varet nga Expression Blend SDK

- Serveri mund te hostohet per testim pa perdorur windows service ne FileSharing.Serveri.Console

- Kur hostohet ne console, serveri ruan fajllat ne C:\Users\<EMRI>\FileSharing Serveri

- Kur hostohet ne Windows Service fajllat ruhen ne C:\ProgramData\FileSharing Serveri

- User llogarite per klient krijohen permes command line ne FileSharing.Klienti.Console i cili ben kerkesat ne server

- Konektimi i Klienti.Console behet duke dhene IP dhe portin e kanalit komunikues te serverit, psh. "127.0.0.1:7000"

- Pas konektimit shfrytezues krijohet me komanden "krijo_user" psh. "krijo_user edon 123"

- Shfrytezuesit e krijuar qasen ne GUI aplikacionin e gjendur ne FileSharing.Klienti.UI

- Klienti UI kerkon fajllin "server_info.xml" ne folderin ku gjendet .exe e saj, nje fajll shembull eshte perfshire ne root folder