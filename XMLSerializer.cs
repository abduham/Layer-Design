using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.IO;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;

namespace EMR.PSS.SOFT.CRM.Utility
{
    public static class XMLSerializer
    {
        /// <summary>
        ///     Gets the XML from list.
        /// </summary>
        /// <param name="listObject">The list object.</param>
        /// <returns></returns>
        public static string GetXmlFromList(object value)
        {
            if (value != null)
            {
                var swListObject = new StringWriter(new StringBuilder(), CultureInfo.InvariantCulture);
                (new XmlSerializer(value.GetType())).Serialize(swListObject, value);
                return RemoveAllNamespaces(swListObject.ToString());
            }
            else { return null; }
        }

        public static string GetXmlRemoveEmptyElement(object value)
        {
            if (value != null)
            {
                var swListObject = new StringWriter(new StringBuilder(), CultureInfo.InvariantCulture);
                (new XmlSerializer(value.GetType())).Serialize(swListObject, value);
                return RemoveEmptyElementNamespaces(swListObject.ToString());
            }
            else { return null; }
        }

        public static string RemoveEmptyElements(string inputXmlValue)
        {
            if (string.IsNullOrEmpty(inputXmlValue)) return string.Empty;
            var docXml = XElement.Parse(inputXmlValue);
            docXml.Descendants().ToList().Where(d => d.Value.Length == 0).Remove();
            return docXml.ToString();
        }

        /// <summary>
        ///     Removes all namespaces.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns></returns>
        /// 

        //Just commented-10/12/15

        public static string RemoveEmptyElementNamespaces(string xmlDocument)
        {
            return RemoveAllNamespaces(XElement.Parse(RemoveEmptyElements(xmlDocument))).ToString();
        }

        public static string RemoveAllNamespaces(string xmlDocument)
        {
            return RemoveAllNamespaces(XElement.Parse(xmlDocument)).ToString();
        }

        public static XDocument LoadXml(string path)
        {
            XDocument document = XDocument.Load(path);

            return document;
        }


        /// <summary>
        ///     Removes all namespaces.
        /// </summary>
        /// <param name="xmlDocument">The XML document.</param>
        /// <returns></returns>
        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (xmlDocument != null)
            {
                if (!xmlDocument.HasElements)
                {
                    var xElement = new XElement(xmlDocument.Name.LocalName)
                    {
                        Value = xmlDocument.Value
                    };
                    return xElement;
                }
                return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(RemoveAllNamespaces));
            }
            else { return null; }
        }

        /// <summary>
        ///     Deserializes the parameters.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="document">The document.</param>
        /// <returns></returns>
        public static Collection<T> DeserializeParams<T>(XNode document)
        {
            if (document == null) return null;
            var serializer = new XmlSerializer(typeof(Collection<T>));
            XmlReader reader = document.CreateReader();
            var result = (Collection<T>)serializer.Deserialize(reader);
            reader.Close();
            return result;
        }

        /// <summary>
        ///     Deserializes the specified XML.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static T Deserialize<T>(string xml)
        {
            var xs = new XmlSerializer(typeof(T));
            return (T)xs.Deserialize(new StringReader(xml));
        }

        /// <summary>
        ///     Deserializes the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="toType">To type.</param>
        /// <returns></returns>
        public static object Deserialize(string input, Type toType)
        {
            var ser = new XmlSerializer(toType);
            using (var sr = new StringReader(input))
            {
                return ser.Deserialize(sr);
            }
        }

        /// <summary>
        ///     Deserializes from string.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static T DeserializeFromString<T>(string value)
        {
            var deserializer = new XmlSerializer(typeof(T));
            TextReader tr = new StringReader(value);
            var outObject = (T)deserializer.Deserialize(tr);
            return outObject;
        }

        /// <summary>
        ///     Generics the de serialize.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml">The XML.</param>
        /// <returns></returns>
        public static Collection<T> GenericDeserialize<T>(string xml)
        {
            if (xml == null) return null;
            var serializer = new XmlSerializer(typeof(Collection<T>)); //Creating XmlSerializer for the object
            TextReader xmltextReader = new StringReader(xml); //Geting XMl from the variable. 
            var deserialize = (Collection<T>)serializer.Deserialize(xmltextReader); //Deserialize back to object from XML
            xmltextReader.Close();
            return deserialize;
        }

        /// <summary>
        /// Serialize an object to xml stirng
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="toSerialize"></param>
        /// <returns></returns>
        public static string SerializeObject<T>(this T toSerialize)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
            using (StringWriter textWriter = new StringWriter(CultureInfo.InvariantCulture))
            {
                xmlSerializer.Serialize(textWriter, toSerialize);
                return textWriter.ToString();
            }
        }
    }
}
