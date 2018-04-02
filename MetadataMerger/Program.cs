using System;
using System.Collections.Generic;
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

            var files = Directory.GetDirectories(dataFolder).SelectMany(dir => Directory.GetFiles(dir, "metadata.json"));
            IEnumerable<string> GetLines()
            {
                var headerAppened = false;
                foreach (var file in files)
                {
                    var content = File.ReadAllText(file);
                    var metadata = JObject.Parse(content);
                    var properties = metadata.Children<JProperty>();
                    if (!headerAppened)
                    {
                        yield return string.Join('\t', properties.Select(property => property.Name));
                        headerAppened = true;
                    }

                    yield return string.Join(
                        '\t',
                        properties.Select(
                            property =>
                            property.Value.ToString()
                            .Replace('\t', ' ')
                        )
                    );
                }
            }

            try
            {
                File.WriteAllLines(outputFile, GetLines());
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
