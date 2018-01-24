//**************Coded by Deshan Alahakoon**************
//**************         Safire        ****************
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Markup;
using System.Xml.Serialization;

namespace Safire.Core
{
    public class SerialData
    {


	    public static string s = Assembly.GetExecutingAssembly().Location + "\\";
        public static T XmlDeserializeObject<T>(string xml)
        {
            try
            {
            
                if (File.Exists(xml))
                {
                    var stream = new FileStream(xml, FileMode.Open);
                    var serializer = new XmlSerializer(typeof(T));
                    var xs = (T)serializer.Deserialize(stream);
                    return xs;
                }
                else
                {
                    return default(T);
                }
            }
            catch
            {
                return default(T);
            }
        }
        public static T AbsoluteXmlDeserializeObject<T>(string xml)
        {
            try
            { 
                if (File.Exists(xml))
                {
                    var stream = new FileStream(xml, FileMode.Open);
                    var serializer = new XmlSerializer(typeof(T));
                    var xs = (T)serializer.Deserialize(stream);
                    return xs;
                }
                else
                {
                    return default(T);
                }
            }
            catch
            {
                return default(T);
            }
        }
        public static void XmlSerializeObject<T>(string filename, T obj)
        {
            try
            {
                filename = s + filename + ".xml";
                Stream stream = File.Open(filename, FileMode.Create);
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stream, obj);
                stream.Close();
            }
            catch
            {
            }
            ;
        }
        public static void AbsoluteXmlSerializeObject<T>(string filename, T obj)
        {
            try
            {
                Stream stream = File.Open(filename, FileMode.Create);
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(stream, obj);
                stream.Close();
            }
            catch
            {
            }
            ;
        }
        public static void BinarySerializeObject<T>(string filename, T objectToSerialize)
        {
            try
            {
                filename = s + filename + ".bin";
                Stream stream = File.Open(filename, FileMode.Create);
                var bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, objectToSerialize);
                stream.Close();
            }
            catch
            {
            }
            ;
        }

        public static T BinaryDeSerializeObject<T>(string filename)
        {
            try
            {
                if (!File.Exists(filename)) return default(T);
                Stream stream = File.Open(filename, FileMode.Open);
                var bFormatter = new BinaryFormatter();
                var objectToSerialize = (T)bFormatter.Deserialize(stream);
                return objectToSerialize;
            }
            catch
            {
                return default(T);
            }
            ;
        }
        public static void AbsBinarySerializeObject<T>(string filename, T objectToSerialize)
        {
            try
            {
				Stream stream = File.Open(filename, FileMode.Create);
                var bFormatter = new BinaryFormatter();
                bFormatter.Serialize(stream, objectToSerialize);
                stream.Close();
            }
            catch (Exception ex)
            {
                Debug.Write(ex.Message);
            }
            ;
        }

        public static T AbsBinaryDeSerializeObject<T>(string filename)
        {
            try
            {
                filename = filename;
                if (!File.Exists(filename)) return default(T);
                Stream stream = File.Open(filename, FileMode.Open);
                var bFormatter = new BinaryFormatter();
                var objectToSerialize = (T)bFormatter.Deserialize(stream);
                return objectToSerialize;
            }
            catch
            {
                // MessageBox.Show(ex.Message);
                return default(T);
            }
            ;
        }
        #region XAML

        public static void WriteXaml<T>(string file, T obj)
        {
            try
            {
                file = s + file + ".xaml";

                Stream stream = File.Open(file, FileMode.Create);
                XamlWriter.Save(obj, stream);
                stream.Close();
            }
            catch
            {
            }
            ;
        }

        public static T LoadXaml<T>(string file)
        {
            try
            {
                file = s + file + ".xaml";
                if (File.Exists(file))
                {
                    Stream stream = File.Open(file, FileMode.Open);
                    return (T)XamlReader.Load(stream);
                }
                return default(T);
            }
            catch
            {
                return default(T);
            }
            ;
        }

        #endregion
    }
}