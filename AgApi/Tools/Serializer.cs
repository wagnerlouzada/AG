using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Serialization
{
    public class Serializer
    {
        public static object Deserialize(Type classtype, String FileName)
        {
            XmlSerializer deserializer = new XmlSerializer(classtype);
            object obj = null;
            using (TextReader reader = new StreamReader(FileName))
            {
                obj = deserializer.Deserialize(reader);
                reader.Close();
            }
            return obj;
        }

        public static object Deserialize(Type classtype, MemoryStream dados)
        {
            XmlSerializer deserializer = new XmlSerializer(classtype);
            object obj = null;
            using (TextReader reader = new StreamReader(dados))
            {
                obj = deserializer.Deserialize(reader);
                reader.Close();
            }
            return obj;
        }

        public static String SerializeObject(Object pObject, Encoding encoding)
        {
            String XmlizedString = null;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(pObject.GetType());
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, encoding);
                xs.Serialize(xmlTextWriter, pObject);
                memoryStream.Position = 0;
                XmlizedString = encoding.GetString(memoryStream.ToArray());
                memoryStream.Close();
            }
            return XmlizedString;
        }

        public static byte[] SerializeObjectByteArray(Object pObject, Encoding encoding)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                XmlSerializer xs = new XmlSerializer(pObject.GetType());
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, encoding);
                xs.Serialize(xmlTextWriter, pObject);
                memoryStream.Position = 0;
                return memoryStream.ToArray();
            }
        }

        public static XmlNode SerializeObjectToXmlNode(Object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Argument cannot be null");

            XmlNode resultNode = null;
            XmlSerializer xmlSerializer = new XmlSerializer(obj.GetType());
            using (MemoryStream memoryStream = new MemoryStream())
            {
                try
                {
                    xmlSerializer.Serialize(memoryStream, obj);
                }
                catch (InvalidOperationException)
                {
                    return null;
                }
                memoryStream.Position = 0;
                XmlDocument doc = new XmlDocument();
                doc.Load(memoryStream);
                resultNode = doc.DocumentElement;
            }
            return resultNode;
        }

    }
}
