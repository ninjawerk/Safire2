using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace Safire.Core.Services
{
    internal class User
    {
        public static string GetRealName(string un)
        {
            string url =
                "http://ws.audioscrobbler.com/2.0/?method=user.getinfo&user=" + un +
                "&api_key=1d3afe8559c38d819987e7bf617222f5";
            string xml;
            using (var webClient = new WebClient())
            {
                xml = webClient.DownloadString(url);
            }
            XElement root = XElement.Parse(xml);
            var user = (string)
                (from el in root.Descendants("realname")
                    select el).First();
            return user;
        }
    }
}