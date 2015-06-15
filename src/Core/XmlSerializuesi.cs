using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace FileSharing.Core
{
    public static class XmlSerializuesi<T>
    {
        private static readonly XmlSerializer Serializuesi = new XmlSerializer(typeof(T));

        public static byte[] SerializoBajt(T objekti)
        {
            using (var memoryStream = new MemoryStream())
            {
                var xmlWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);
                Serializuesi.Serialize(xmlWriter, objekti);
                return memoryStream.ToArray();
            }
        }

        public static string Serializo(T objekti)
        {
            return Encoding.UTF8.GetString(SerializoBajt(objekti));
        }

        public static T DeserializoBajt(byte[] bajtat)
        {
            using (var memoryStream = new MemoryStream(bajtat))
            {
                return (T)Serializuesi.Deserialize(memoryStream);
            }
        }

        public static T Deserializo(string teksti)
        {
            using (var textReader = new StringReader(teksti))
            {
                return (T)Serializuesi.Deserialize(textReader);
            }
        }
    }
}
