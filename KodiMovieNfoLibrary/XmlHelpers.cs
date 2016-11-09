using System;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace KodiMovieNfoLibrary
{
    public static class XmlHelpers
    {
        public static string XmlSerializeToString(this object objectInstance)
        {
            var ns = new XmlSerializerNamespaces();

            //Add an empty namespace and empty value
            ns.Add("", "");
            var serializer = new XmlSerializer(objectInstance.GetType());

            var settings = new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            using (var stream = new StringWriter())
            using (var writer = XmlWriter.Create(stream, settings))
            {
                serializer.Serialize(writer, objectInstance, ns);
                return stream.ToString();
            }
        }

        public static T XmlDeserializeFromString<T>(this string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }

        public static object XmlDeserializeFromString(this string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }
    }
}