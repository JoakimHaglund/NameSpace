using NameSpace.Dtos;
using NameSpace.Models;
using System.Text.RegularExpressions;
using System.Text;

namespace NameSpace.Services
{
    public class FileReader
    {
        public async Task<HashSet<NameInfoDto>> ReadCSV(IFormFile file)
        {
            var resultSet = new HashSet<NameInfoDto>(); // 🔥 HashSet istället för dictionary

            using (var reader = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
            {
                // Skippa första raden (headers)
                _ = await reader.ReadLineAsync();

                while (!reader.EndOfStream)
                {
                    var line = await reader.ReadLineAsync();

                    if (line == null) continue;

                    string delimiterPattern = string.Join("", Array.ConvertAll([',', ';', ':', '.'], c => Regex.Escape(c.ToString())));
                    string pattern = $@"(?:\""[^\""]*\"")|([^\""{delimiterPattern}]+)";
                    var match = Regex.Matches(line, pattern);

                    if (match.Count == 3)
                    {
                        var name = match[0].Value.ToString();
                        var antal = int.Parse(match[1].Value.Replace(" ", ""));
                        var gender = int.Parse(match[2].Value);

                        var dto = new NameInfoDto
                        {
                            Name = name,
                            Antal = antal,
                            DescriptionOfName = string.Empty,
                            Gender = gender
                        };

                        // Här kollar vi om den redan finns i setet
                        if (resultSet.TryGetValue(dto, out var existing))
                        {
                            // uppdatera befintlig
                            existing.Antal += dto.Antal;
                        }
                        else
                        {
                            // annars lägg till
                            resultSet.Add(dto);
                        }
                    }
                }
            }

            return resultSet;
        }

    }
}
