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
    }
}