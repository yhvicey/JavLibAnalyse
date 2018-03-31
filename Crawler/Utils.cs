using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Crawler
{
    public static class Utils
    {
        public static byte[] GetResponseByteContent(Uri url)
            => Encoding.Default.GetBytes(GetResponseContent(url));

        public static string GetResponseContent(Uri url)
        {
            using (var client = new HttpClient())
            {
                return client.GetStringAsync(url).Result;
            }
        }
    }
}
