using System;
using System.IO;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace libc_def.Web
{
	class Deserializer<T>
	{
		public static T  Deserialize(string data)
		{
			try
			{
				JavaScriptSerializer serializer = new JavaScriptSerializer();

				return serializer.Deserialize<T>(data);
			}
			catch (Exception )
			{
				return default(T);
			}
		}
		public static T XmlDeserializeObject(string xml)
		{
			try
			{
				StringReader TheStream = new StringReader(xml);
					var serializer = new XmlSerializer(typeof(T));
					var xs = (T)serializer.Deserialize(TheStream);
					return xs;
			 
			}
			catch
			{
				return default(T);
			}
		}
	}
}
