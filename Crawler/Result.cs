using System;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace Crawler
{
    public class Result
    {
        public string Identifier { get; set; }
        public DateTime Date { get; set; }
        public int Length { get; set; }
        public string Director { get; set; }
        public string Maker { get; set; }
        public string Label { get; set; }
        public int Review { get; set; }
        public string Genres { get; set; }
        public string Cast { get; set; }
        public Image<Rgba32> Image { get; set; }
    }
}