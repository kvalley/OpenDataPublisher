using System;
using System.Globalization;
using System.Net;
using System.Web;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using System.Text;

using Odp.Data.DataSets;
using Odp.Data.Sql;
using Odp.Data.Views;

namespace Odp.Data
{
    public class AnalyticInfoStorage
    {
        private static int XAttrToInt(XAttribute attr)
        {
            try
            {
                return Convert.ToInt32(attr.Value);
            }
            catch
            {
                return 0;
            }
        }

        private static double XAttrToDouble(XAttribute attr)
        {
            try
            {
                return Convert.ToDouble(attr.Value);
            }
            catch
            {
                return 0;
            }
        }

        private static string XAttrToStr(XAttribute attr)
        {
            try
            {
                return attr.Value;
            }
            catch
            {
                return "";
            }
        }

        private static DateTime XAttrToDateTime(XAttribute attr)
        {
            try
            {
                return Convert.ToDateTime(attr.Value);
            }
            catch
            {
                return new DateTime();
            }
        }

        public static IQueryable<AnalyticInfo> GetAllAnalyticInfos(string baseUri)
        {
            var uri = new Uri(baseUri + "AnalyticInfo");
            var webRequest = HttpWebRequest.Create(uri);
            var response = webRequest.GetResponse();
            var responseStream = response.GetResponseStream();
            var feed = XElement.Load(XmlReader.Create(responseStream));
            var qlist = feed.Elements("entityset");
            var list = new List<AnalyticInfo>();
            foreach (var el in qlist)
            {
                var ai = new AnalyticInfo(XAttrToStr(el.Attribute("name")));
                ai.views_total = XAttrToInt(el.Attribute("views_total"));
                ai.views_today = XAttrToInt(el.Attribute("views_today"));
                ai.views_average = XAttrToInt(el.Attribute("views_average"));
                ai.last_viewed = XAttrToDateTime(el.Attribute("last_viewed"));
                ai.PositiveVotes = XAttrToInt(el.Attribute("positive_votes"));
                ai.NegativeVotes = XAttrToInt(el.Attribute("negative_votes"));
                list.Add(ai);
            }
            return list.AsQueryable<AnalyticInfo>();
        }

        public static AnalyticInfo GetAnalyticInfo(string uri, string key)
        {
            return (from info in GetAllAnalyticInfos(uri) where info.RowKey == key select info).FirstOrDefault();
        }

        public static void AddAnalyticInfo(string baseUri, string key)
        {
            var uri = new Uri(baseUri + "AnalyticInfo/?new=" + key);
            var webRequest = HttpWebRequest.Create(uri);
            var response = webRequest.GetResponse();
            var responseStream = response.GetResponseStream();
        }

        public static void Update(string baseUri, AnalyticInfo value)
        {
            var fmt = new NumberFormatInfo() { NumberDecimalSeparator = "." };
            var uri = new Uri(
                baseUri + "AnalyticInfo/?" +
                "update=" + value.RowKey + "&" +
                "views_total=" + value.views_total + "&" +
                "views_today=" + value.views_today + "&" +
                "views_average=" + value.views_average.ToString(fmt) + "&" +
                "last_viewed=" + value.last_viewed.ToString("yyyy-MM-dd HH:mm:ss") + "&" +
                "positive_votes=" + value.PositiveVotes + "&" +
                "negative_votes=" + value.NegativeVotes
                );
            var webRequest = HttpWebRequest.Create(uri);
            var response = webRequest.GetResponse();
            var responseStream = response.GetResponseStream();
        }

        public static void AddDownloadAnalytic(string baseUri, String key)
        {
            var uri = new Uri(baseUri + "DownloadAnalyticInfo/?new=" + key);
            var webRequest = HttpWebRequest.Create(uri);
            var response = webRequest.GetResponse();
            var responseStream = response.GetResponseStream();
        }
    }
}
