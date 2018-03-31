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
            try
            {
                var url = new Uri($"{Config.RootUrl}/?v={task}");
                var content = Utils.GetResponseContent(url);
                if (content == null)
                {
                    Dispatcher.ReAddProcessorTask(task);
                    return null;
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(content);
                var root = doc.DocumentNode;

                // Extract title
                var titleNode = root?.GetChildElementById("video_title");
                var title = titleNode?.GetChildElement("h3/a")?.InnerText;

                // Extract video info
                var videoInfoNode = root?.GetChildElementById("video_info");
                var id = videoInfoNode?.GetChildElementById("video_id")?.GetChildElement("table/tr/td[@class='text']")?.InnerText;
                var date = videoInfoNode?.GetChildElementById("video_date")?.GetChildElement("table/tr/td[@class='text']")?.InnerText;
                var length = videoInfoNode?.GetChildElementById("video_length")?.GetChildElement("table/tr/td/span")?.InnerText;
                var director = videoInfoNode?.GetChildElementById("video_director")?.GetChildElement("table/tr/td/span/a")?.InnerText;
                var maker = videoInfoNode?.GetChildElementById("video_maker")?.GetChildElement("table/tr/td/span/a")?.InnerText;
                var label = videoInfoNode?.GetChildElementById("video_label")?.GetChildElement("table/tr/td/span/a")?.InnerText;
                var review = videoInfoNode?.GetChildElementById("video_review")?.GetChildElement("table/tr/td/span[@class='score']")?.InnerText;
                var genres = videoInfoNode?.GetChildElementById("video_genres")?.GetChildElements("table/tr/td/span/a")?.Select(node => node.InnerText);
                var cast = videoInfoNode?.GetChildElementById("video_cast")?.GetChildElements("table/tr/td/span/span/a")?.Select(node => node.InnerText);

                // Get image
                Image<Rgba32> image = null;
                try
                {
                    var imageNode = root?.GetChildElementById("video_jacket_img");
                    var imgUrl = new Uri($"{Config.RootUrl}/{imageNode.GetAttributeValue("src", "../img/noimagepl.gif")}");
                    if (imgUrl.Segments.Last() != "noimagepl.gif")
                        image = Image.Load(Utils.GetResponseByteContent(imgUrl));
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to get image. Task: {task}", ex);
                }

                return new Result
                {
                    VId = task,
                    Title = title,
                    Identifier = id,
                    Date = date == null ? default(DateTime) : DateTime.Parse(date),
                    Length = length == null ? 0 : int.Parse(length),
                    Director = director,
                    Maker = maker,
                    Label = label,
                    Review = string.IsNullOrWhiteSpace(review) ? 0 : double.Parse(review.Trim('(', ')')),
                    Genres = genres == null ? null : string.Join(";", genres),
                    Cast = cast == null ? null : string.Join(";", cast),
                    Image = image
                };
            }
            catch (Exception ex)
            {
                Logger.Error($"Error occured while processing. Task: {task}", ex);
                return null;
            }
        }
    }
}