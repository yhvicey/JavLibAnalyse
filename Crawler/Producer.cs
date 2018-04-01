using System;
using System.Linq;
using HtmlAgilityPack;

namespace Crawler
{
    public static class Producer
    {
        public static bool Produce(string genres, int page)
        {
            try
            {
                var url = new Uri($"{Config.RootUrl}/vl_genre.php?&g={genres}&page={page}");
                var content = Utils.GetResponseContent(url);
                if (content == null)
                {
                    Dispatcher.AddProducerTask(genres, page);
                    return false;
                }

                var doc = new HtmlDocument();
                doc.LoadHtml(content);
                var root = doc.DocumentNode;

                // Get video list
                var videosNode = root?.GetChildElement("//*[@class='videos']");
                if (videosNode == null)
                    return true;
                var videoList = videosNode?.GetChildElements("div[@class='video']")
                    ?.Select(node => node.GetAttributeValue("id", null))
                    ?.Where(id => id != null)
                    ?.Select(id => id.Remove(0, 4));
                if (videoList == null)
                    return true;
                foreach (var id in videoList)
                    Dispatcher.AddProcessorTask(id);

                Dispatcher.FinishProducerTask(genres, page);
                Dispatcher.AddProducerTask(genres, page + 1);
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error occured while producing. Genres: {genres}, Page: {page}.", ex);
                Dispatcher.AddProducerTask(genres, page);
                return false;
            }
        }
    }
}
