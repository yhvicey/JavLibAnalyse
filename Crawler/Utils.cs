using System;
using System.Net.Http;

namespace Crawler
{
    public static class Utils
    {
        public static byte[] GetResponseByteContent(Uri url, string referer = null)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Set-Cookies", "app_uid=ygb1XVq/kuInDMYcBvBTAg==");
                client.DefaultRequestHeaders.Add("Accept", "image/webp,image/apng,image/*,*/*;q=0.8");
                client.DefaultRequestHeaders.Add("DNT", "1");
                client.DefaultRequestHeaders.Add("Referer", referer);
                client.DefaultRequestHeaders.Add("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.Add("Accept-Language", "en-US,en;q=0.9,zh-CN;q=0.8,zh;q=0.7");

                return client.GetByteArrayAsync(url).Result;
            }
        }

        public static string GetResponseContent(Uri url)
        {
            using (var client = new HttpClient())
            {
                return client.GetStringAsync(url).Result;
            }
        }
    }
}
