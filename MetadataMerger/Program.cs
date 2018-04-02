using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace MetadataMerger
{
    class Program
    {
        static int Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: dotnet MetadataMerger.dll <data folder> <output file>");
                return 0;
            }
            var dataFolder = args[0];
            var outputFile = args[1];

            if (!Directory.Exists(dataFolder))
            {
                Console.WriteLine("Invalid data folder!");
                return 1;
            }

            try
            {
                var files = Directory.GetDirectories(dataFolder).SelectMany(dir => Directory.GetFiles(dir, "metadata.json"));
                var lines = files.Select(
                    file =>
                    {
                        var content = File.ReadAllText(file);
                        var metadata = JObject.Parse(content);
                        return string.Join(
                            '\t',
                            metadata.Children<JProperty>()
                            .Select(
                                property =>
                                property.Value.ToString()
                                .Replace('\t', ' ')
                            )
                        );
                    });
                File.WriteAllLines(outputFile, lines);
                return 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return -1;
            }
        }
    }
}
