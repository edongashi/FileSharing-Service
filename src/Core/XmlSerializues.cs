using System.IO;
using System.Xml.Serialization;

namespace FileSharing.Core
{
    public static class XmlSerializues<T>
    {
        private static readonly XmlSerializer Serializuesi = new XmlSerializer(typeof(T));

        public static string Serializo(T objekti)
        {
            using (var textWriter = new StringWriter())
            {
                Serializuesi.Serialize(textWriter, objekti);
                return textWriter.ToString();
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
