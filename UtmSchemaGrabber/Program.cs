using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text.RegularExpressions;

namespace UtmSchemaGrabber
{
    internal class Program
    {
        public static string AssemblyVersion => Assembly.GetExecutingAssembly().GetName().Version.ToString();

        private static void Main(string[] args)
        {
            Console.WriteLine($"UtmSchemaGrabber, Версия {AssemblyVersion}\n");

            if (args.Length < 1)
            {
                Console.WriteLine("Использование: UtmSchemaGrabber.exe http://<UtmAddress>:<port>");
                Console.WriteLine("где:\n\t<UtmAddress> - полный адрес УТМ");
                Console.WriteLine("\t<port> - порт, на котором работает УТМ\n");
                Console.WriteLine("Пример: UtmSchemaGrabber.exe http://localhost:8080");

                return;
            }

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
                using (var client = new HttpClient())
                {
                    var mainPage = client.GetStringAsync(new Uri(args[0])).GetAwaiter().GetResult();
                    var matches = regexList.Matches(WebUtility.HtmlDecode(mainPage));

                    if (matches.Count == 0)
                    {
                        Console.WriteLine("По заданной ссылке не найдено ни одной схемы XSD");
                        return;
                    }

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

                    Console.WriteLine("Сохранение файлов...\n");

                    foreach (var file in listOfFiles)
                    {
                        Console.WriteLine(file.FileName);
                        var html = client.GetStringAsync(file.Url).GetAwaiter().GetResult();
                        var match = regexPre.Match(html).Groups[1].Value;
                        File.WriteAllText(rootDir.FullName + file.FileName, WebUtility.HtmlDecode(match));
                    }
                }
            }
            catch (Exception ex)
            {
                do
                {
                    Console.WriteLine(ex.Message);
                    ex = ex.InnerException;
                } while (ex != null);
            }
        }
    }
}
