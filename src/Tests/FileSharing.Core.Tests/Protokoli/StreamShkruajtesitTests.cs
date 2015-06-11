using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using FileSharing.Core.Protokoli;
using FileSharing.Core.Protokoli.Sherbimet;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileSharing.Core.Tests.Protokoli
{
    [TestClass]
    public class StreamShkruajtesitTests
    {
        [TestMethod]
        public async Task StreamShkruajMesazh()
        {
            var streamShkruajtes = new StreamShkruajtes();
            var memoryStream = new MemoryStream();

            byte mesazhiGjatesia = 1 + 5;
            var framedMadhesia = mesazhiGjatesia + Konfigurimi.PrefixGjatesia;
            var mesazhi = new Mesazh(Header.Ok, new byte[] { 1, 2, 3, 10, 15 });

            await streamShkruajtes.ShkruajMesazhAsync(memoryStream, mesazhi);
            Assert.AreEqual(framedMadhesia, (int)memoryStream.Length);

            var expectedMesazhi = new byte[] { mesazhiGjatesia, 0, 0, 0, Header.Ok, 1, 2, 3, 10, 15 };

            memoryStream.Position = 0;
            var buffer = new byte[framedMadhesia];
            memoryStream.Read(buffer, 0, buffer.Length);
            for (var i = 0; i < expectedMesazhi.Length; i++)
            {
                Assert.AreEqual(expectedMesazhi[i], buffer[i]);
            }
        }

        [TestMethod]
        public async Task StreamLexoMesazh()
        {
            var streamShkruajtes = new StreamShkruajtes();

            // 200-shat nuk duhen te lexohen
            var mesazhiTest = new byte[] { 7, 0, 0, 0, Header.Ok, 1, 2, 3, 10, 15, 17, 200, 200, 200, 200 };
            var mesazhiBajtat = new byte[] { 1, 2, 3, 10, 15, 17 };

            var memoryStream = new MemoryStream(mesazhiTest) { Position = 0 };

            var mesazhiPranuar = await streamShkruajtes.LexoMesazhAsync(memoryStream);
            Assert.AreEqual(7, mesazhiPranuar.Gjatesia);
            Assert.AreEqual(Header.Ok, mesazhiPranuar.Header);

            for (int i = 0; i < 6; i++)
            {
                Assert.AreEqual(mesazhiBajtat[i], mesazhiPranuar.TeDhenat[i]);
            }
        }

        [TestMethod]
        public async Task StreamTransferDergo()
        {
            var streamTransferues = new StreamTransferShkruajtes();

            var bajtat = new byte[] { 5, 3, 7 };

            // ReSharper disable once PossibleNullReferenceException
            var hash = HashAlgorithm.Create(Konfigurimi.HashAlgoritmi).ComputeHash(bajtat);

            var streamDerguesi = new MemoryStream(bajtat);
            var streamPranuesi = new MemoryStream();

            await streamTransferues.DergoStreamAsync(3, streamDerguesi, streamPranuesi);

            streamPranuesi.Position = 0;
            Assert.AreEqual(5, streamPranuesi.ReadByte());
            Assert.AreEqual(3, streamPranuesi.ReadByte());
            Assert.AreEqual(7, streamPranuesi.ReadByte());
            foreach (var b in hash)
            {
                Assert.AreEqual(b, streamPranuesi.ReadByte());
            }
        }

        [TestMethod]
        public async Task StreamTransferPrano()
        {
            var streamTransferues = new StreamTransferShkruajtes();

            var bajtat = new byte[] { 5, 3, 7 };

            // ReSharper disable once PossibleNullReferenceException
            var hash = HashAlgorithm.Create(Konfigurimi.HashAlgoritmi).ComputeHash(bajtat);

            var streamDerguesi = new MemoryStream();
            streamDerguesi.Write(bajtat, 0, bajtat.Length);
            streamDerguesi.Write(hash, 0, hash.Length);
            var streamPranuesi = new MemoryStream();

            streamDerguesi.Position = 0;
            await streamTransferues.PranoStreamAsync((int)streamDerguesi.Length, streamDerguesi, streamPranuesi);

            Assert.AreEqual(3, streamPranuesi.Length);
            streamPranuesi.Position = 0;
            Assert.AreEqual(5, streamPranuesi.ReadByte());
            Assert.AreEqual(3, streamPranuesi.ReadByte());
            Assert.AreEqual(7, streamPranuesi.ReadByte());
        }

        [TestMethod]
        [ExpectedException(typeof(HashFailException))]
        public async Task StreamTransferPranoHashFail()
        {
            var streamTransferues = new StreamTransferShkruajtes();

            var bajtat = new byte[] { 5, 3, 7 };

            // ReSharper disable once PossibleNullReferenceException
            var hash = HashAlgorithm.Create(Konfigurimi.HashAlgoritmi).ComputeHash(bajtat);

            hash[5]++;

            var streamDerguesi = new MemoryStream();
            streamDerguesi.Write(bajtat, 0, bajtat.Length);
            streamDerguesi.Write(hash, 0, hash.Length);
            var streamPranuesi = new MemoryStream();

            streamDerguesi.Position = 0;
            await streamTransferues.PranoStreamAsync((int)streamDerguesi.Length, streamDerguesi, streamPranuesi);

            Assert.Fail();
        }
    }
}
