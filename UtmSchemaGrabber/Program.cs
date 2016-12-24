using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;

namespace UtmSchemaGrabber
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length < 1)
            {
                Console.WriteLine("\nИспользование: UtmSchemaGrabber.exe http://<UtmAddress>:<port>");
                Console.WriteLine("где:\n\t<UtmAddress> - полный адрес УТМ");
                Console.WriteLine("\t<port> - порт, на котором работает УТМ\n");
                Console.WriteLine("Пример: UtmSchemaGrabber.exe http://localhost:8080");

                return;
            }

            var client = new HttpClient();
            var regexList = new Regex("\\.load\\('info/(?<Folder>.+?)/(?<Filename>(?:.+?)(?:'|\\.xsd))",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.Singleline
                    | RegexOptions.CultureInvariant
                    | RegexOptions.IgnorePatternWhitespace
                    | RegexOptions.Compiled
                    );

            var regexPre = new Regex("<pre>(.*?)</pre>",
                    RegexOptions.IgnoreCase
                    | RegexOptions.Multiline
                    | RegexOptions.Singleline
                    | RegexOptions.CultureInvariant
                    | RegexOptions.IgnorePatternWhitespace
                    | RegexOptions.Compiled
                    );

            try
            {
                var mainPage = client.GetStringAsync(new Uri(args[0])).GetAwaiter().GetResult();

                var matches = regexList.Matches(WebUtility.HtmlDecode(mainPage));

                var listOfFiles = matches.Cast<Match>()
                    .Where(x => x.Groups["Filename"].Value.EndsWith(".xsd", StringComparison.InvariantCultureIgnoreCase))
                    .Select(x => new
                    {
                        Url = $"{args[0]}/info/{x.Groups["Folder"]}/{x.Groups["Filename"]}",
                        FileName = $@"\{x.Groups["Folder"]}\{x.Groups["Filename"]}"
                    })
                    .ToList();

                var rootDir = Directory.CreateDirectory("EgaisXsd");
                var subDirs = matches.Cast<Match>()
                    .Select(x => x.Groups["Folder"].Value)
                    .Distinct()
                    .Where(x => x != "db" && x != "certificate");

                foreach (var folder in subDirs)
                {
                    Directory.CreateDirectory($@"{rootDir.FullName}\{folder}");
                }

                foreach (var file in listOfFiles)
                {
                    var html = client.GetStringAsync(file.Url).GetAwaiter().GetResult();
                    var match = regexPre.Match(html).Groups[1].Value;
                    File.WriteAllText(rootDir.FullName + file.FileName, WebUtility.HtmlDecode(match));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
