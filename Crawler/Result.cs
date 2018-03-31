using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Crawler
{
    public class Result
    {
        public string VId { get; set; }
        public string Title { get; set; }
        public string Identifier { get; set; }
        public DateTime Date { get; set; }
        public int Length { get; set; }
        public string Director { get; set; }
        public string Maker { get; set; }
        public string Label { get; set; }
        public double Review { get; set; }
        public string Genres { get; set; }
        public string Cast { get; set; }
        public Image<Rgba32> Image { get; set; }
    }
}