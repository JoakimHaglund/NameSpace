using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace FormatNameLists
{
    class ReadCSV
    {
        private string Path { get; set; }
        private bool HasHeaderRow { get; set; } = false;
        private char[]? Delimitors { get; set; }

        private readonly char[] DefaultDelimitors = [',', ';', ':', '.'];
        public ReadCSV(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) throw new ArgumentException("No Path was provided");
            Path = path;
        }
        public ReadCSV HasHeader()
        {
            HasHeaderRow = true;
            return this;
        }
        public ReadCSV SetDelimitor(char delimitor)
        {
            Delimitors = [delimitor];
            return this;
        }
        public ReadCSV SetDelimitors(char[] delimitor)
        {
            Delimitors = delimitor;
            return this;
        }
        public List<T> ReadAsClass<T>() where T : class
        {
            List<T> result = [];
            
            var lines = File.ReadAllLines(Path).ToList();
            foreach (var line in lines) 
            {
                // Gör delimiters till en regex
                string delimiterPattern = new(Delimitors);

                // Regex för att dela på texten men ignorera text inom citattecken
                string pattern = $@"[^""{delimiterPattern}]+(?:\""[^\""]*\""|[^{delimiterPattern}])*";

                var match = Regex.Matches(line, pattern);

                foreach (var item in match)
                {
                    Console.WriteLine(item);
                    //foreach item matched make a list
                }
                    //foreach in T Add it form previus list
                    //error handel if T and item list is unmatched?
            }
            return result;
        }
        public Dictionary<string, string> ReadToDictionary()
        {
            Delimitors ??= DefaultDelimitors; 

            var result = new Dictionary<string, string>();
            var lines = File.ReadAllLines(Path).ToList();
            if (HasHeaderRow) lines.RemoveAt(0);
            foreach (var line in lines)
            {
                // Skapa en regex-sträng för att matcha delimiters som du vill använda
                string delimiterPattern = string.Join("", Array.ConvertAll(Delimitors, c => Regex.Escape(c.ToString())));

                // Regex för att matcha text inom citattecken och även dela vid delimiters
                string pattern = $@"(?:\""[^\""]*\"")|([^\""{delimiterPattern}]+)";

                var match = Regex.Matches(line, pattern);
                if (match.Count() >= 2)
                {
                    var name = match[0].Value.ToString();
                    if (result.ContainsKey(name))
                    {
                        result[name] = (int.Parse(result[name].Replace(" ", "")) + int.Parse(match[1].Value.Replace(" ", ""))).ToString();
                    }
                    else
                    {
                        result.Add(match[0].Value.ToString(), match[1].Value.ToString());
                    }
                }
            }
            return result;
        }
    }
}
