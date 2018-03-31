using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;

namespace Crawler
{
    public static class Saver
    {
        public static bool Save(string id, Result result)
        {
            var resultDir = $"{Config.OutputDir}/{id}";
            try
            {
                if (Directory.Exists(resultDir))
                    Directory.Delete(resultDir, true);
                Directory.CreateDirectory(resultDir);

                var metadataFilePath = $"{resultDir}/metadata.json";

                var metadata = new JObject
                {
                    ["id"] = result.Identifier,
                    ["date"] = result.Date,
                    ["length"] = result.Length,
                    ["director"] = result.Director,
                    ["maker"] = result.Maker,
                    ["label"] = result.Label,
                    ["review"] = result.Review,
                    ["genres"] = result.Genres,
                    ["cast"] = result.Cast
                };

                File.WriteAllText(metadataFilePath, metadata.ToString());

                if (result.Image != null)
                {
                    var imageFilePath = $"{resultDir}/Cover.jpg";
                    try
                    {
                        result.Image.Save(imageFilePath);
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to save image. Image path: {imageFilePath}.", ex);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to save result. Id: {id}, result dir: {resultDir}.", ex);
                return false;
            }
        }
    }
}