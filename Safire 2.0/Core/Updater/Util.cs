using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using MahApps.Metro.Controls.Dialogs;
using Safire.Properties;

namespace Safire.Core.Updater
{
	internal class Util
	{
		static string dir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);

		public   static void NotifyUpdates()
		{
			
		 
		}
	 
		public static string Serialize<T>(T value)
		{

			if (value == null)
			{
				return null;
			}

			XmlSerializer serializer = new XmlSerializer(typeof(T));

			XmlWriterSettings settings = new XmlWriterSettings();
			settings.Encoding = new UnicodeEncoding(false, false); // no BOM in a .NET string
			settings.Indent = false;
			settings.OmitXmlDeclaration = false;

			using (StringWriter textWriter = new StringWriter())
			{
				using (XmlWriter xmlWriter = XmlWriter.Create(textWriter, settings))
				{
					serializer.Serialize(xmlWriter, value);
				}
				return textWriter.ToString();
			}
		}
		public static T Deserialize<T>(string xml)
		{
			if (string.IsNullOrEmpty(xml))
			{
				return default(T);
			}
			XmlSerializer serializer = new XmlSerializer(typeof(T));
			XmlReaderSettings settings = new XmlReaderSettings();
			// No settings need modifying here
			using (StringReader textReader = new StringReader(xml))
			{
				using (XmlReader xmlReader = XmlReader.Create(textReader, settings))
				{
					return (T)serializer.Deserialize(xmlReader);
				}
			}
		}
		public static T BinaryDeSerializeObject<T>(string filename)
		{
			try
			{
				if (!File.Exists(filename)) return default(T);
				var stream = new FileStream(filename, FileMode.Open);
				var serializer = new XmlSerializer(typeof(T));
				var xs = (T)serializer.Deserialize(stream);
				return xs;
			}
			catch
			{
				return default(T);
			}
			;
		}
	}

}
