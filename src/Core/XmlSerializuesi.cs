using System.IO;
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
                Serializuesi.Serialize(memoryStream, objekti);
                return memoryStream.ToArray();
            }
        }

        public static string Serializo(T objekti)
        {
            using (var textWriter = new StringWriter())
            {
                Serializuesi.Serialize(textWriter, objekti);
                return textWriter.ToString();
            }
        }

        public static T DeserializoBajt(byte[] bajtat)
        {
            using (var memoryStream = new MemoryStream(bajtat))
            {
                memoryStream.Position = 0;
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
