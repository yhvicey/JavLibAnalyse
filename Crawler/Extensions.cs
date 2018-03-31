using System;
using System.IO;
using System.Net;
using System.Text;
using HtmlAgilityPack;

namespace Crawler
{
    public static class Extensions
    {
        public static HtmlNode GetChildElement(this HtmlNode node, string path)
            => node.SelectSingleNode(path);

        public static HtmlNodeCollection GetChildElements(this HtmlNode node, string path)
            => node.SelectNodes(path);

        public static HtmlNode GetChildElementById(this HtmlNode node, string id)
            => node.SelectSingleNode($"//*[@id='{id}']");

        public static byte[] GetResponseByteContent(this Uri url)
            => Encoding.Default.GetBytes(url.GetResponseContent());

        public static string GetResponseContent(this Uri url)
        {
            var request = WebRequest.CreateHttp(url);
            request.Method = "GET";
            request.Proxy = WebRequest.DefaultWebProxy;
            using (var response = request.GetResponse())
            {
                using (var reader = new StreamReader(response.GetResponseStream(), Encoding.Default))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}