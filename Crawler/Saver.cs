using System;
using System.IO;
using Newtonsoft.Json.Linq;
using SixLabors.ImageSharp;

namespace Crawler
{
    public static class Saver
    {
        public static bool Save(Result result)
        {
            if (string.IsNullOrWhiteSpace(result?.VId)) throw new ArgumentException(nameof(result), "Invalid result.");

            var resultDir = $"{Config.OutputDir}/{result.VId}";
            try
            {
                if (Directory.Exists(resultDir))
                    Directory.Delete(resultDir, true);
                Directory.CreateDirectory(resultDir);

                var metadataFilePath = $"{resultDir}/metadata.json";

                var metadata = new JObject
                {
                    ["vid"] = result.VId,
                    ["title"] = result.Title,
                    ["id"] = result.Identifier,
                    ["date"] = result.Date.ToString("yyyy-MM-dd"),
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
                Logger.Error($"Failed to save result. Id: {result.VId}, result dir: {resultDir}.", ex);
                return false;
            }
        }
    }
}