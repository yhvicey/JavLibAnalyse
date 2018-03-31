using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Crawler
{
    public static class Processor
    {
        public static Result Process(string task)
        {
            var url = new Uri($"{Config.RootUrl}/?v={task}");
            var content = url.GetResponseContent();
            if (content == null)
            {
                Dispatcher.AddProcessorTask(task);
                return null;
            }

            var doc = new HtmlDocument();
            doc.LoadHtml(content);
            var root = doc.DocumentNode;

            // Extract video info
            var videoInfoNode = root.GetChildElementById("video_info");
            var id = videoInfoNode.GetChildElementById("video_id").GetChildElement("table/tr/td[@class='text']").InnerText;
            var date = videoInfoNode.GetChildElementById("video_date").GetChildElement("table/tr/td[@class='text']").InnerText;
            var length = videoInfoNode.GetChildElementById("video_length").GetChildElement("table/tr/td/span").InnerText;
            var director = videoInfoNode.GetChildElementById("video_director").GetChildElement("table/tr/td/span/a").InnerText;
            var maker = videoInfoNode.GetChildElementById("video_maker").GetChildElement("table/tr/td/span/a").InnerText;
            var label = videoInfoNode.GetChildElementById("video_label").GetChildElement("table/tr/td/span/a").InnerText;
            var review = videoInfoNode.GetChildElementById("video_review").GetChildElement("table/tr/td/span[@class='score']").InnerText;
            var genres = videoInfoNode.GetChildElementById("video_genres").GetChildElements("table/tr/td/span/a").Select(node => node.InnerText);
            var cast = videoInfoNode.GetChildElementById("video_cast").GetChildElements("table/tr/td/span/span/a").Select(node => node.InnerText);

            // Get image
            Image<Rgba32> image = null;
            try
            {
                var imageNode = root.GetChildElementById("video_jacket_img");
                var imgUrl = new Uri($"{Config.RootUrl}/{imageNode.GetAttributeValue("src", "../img/noimagepl.gif")}");
                if (imgUrl.Segments.Last() != "noimagepl.gif")
                    image = Image.Load(imgUrl.GetResponseContent());
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to get image.", ex);
            }

            return new Result
            {
                Identifier = id,
                Date = DateTime.Parse(date),
                Length = int.Parse(length),
                Director = director,
                Maker = maker,
                Label = label,
                Review = string.IsNullOrWhiteSpace(review) ? 0 : int.Parse(review.Trim('(', ')')),
                Genres = string.Join(";", genres),
                Cast = string.Join(";", cast),
                Image = image
            };
        }
    }
}