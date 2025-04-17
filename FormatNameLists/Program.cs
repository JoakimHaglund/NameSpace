// See https://aka.ms/new-console-template for more information
using FormatNameLists;
using System.Collections.Generic;
using System.Diagnostics;
Console.WriteLine("Hello, World!");
var FlicknamnOld = new ReadCSV("E:\\ffswindows\\Desktop\\FlicknamnMedAntal.csv").ReadToDictionary();
var FlicknamnNew = new ReadCSV("E:\\ffswindows\\Desktop\\Flicknamn 21-24.csv").HasHeader().ReadToDictionary();
var PojknamnOld = new ReadCSV("E:\\ffswindows\\Desktop\\PojknamnMedAntal.csv").ReadToDictionary();
var PojknamnNew = new ReadCSV("E:\\ffswindows\\Desktop\\PojkNamn21-2401.csv").HasHeader().ReadToDictionary();

var basePath = "E:\\ffswindows\\Desktop";
Console.WriteLine("MergeNames pojknamn");
var pojknamn = MergeNames(PojknamnOld, PojknamnNew, true);
Console.WriteLine("MergeNames flicknamn");
var flicknamn = MergeNames(FlicknamnOld, FlicknamnNew, false);
var completeList = new List<NameInfo>();
completeList.AddRange(pojknamn);
completeList.AddRange(flicknamn);
var writer = new WriteCSV();
var test = completeList.Where(i => i.Antal > 4).ToList();
Console.WriteLine("WriteCSV pojknamn");
writer.SetFilePath(basePath + "\\Pojknamn.csv").UsePropertynamesAsHeader().SetDelimitor(';').Write(pojknamn);
writer.SetFilePath(basePath + "\\Flicknamn.csv").UsePropertynamesAsHeader().SetDelimitor(';').Write(flicknamn);
writer.SetFilePath(basePath + "\\Alla namn.csv").UsePropertynamesAsHeader().SetDelimitor(';').Write(completeList);
Console.WriteLine("stop");
static List<NameInfo> MergeNamesOld(Dictionary<string,string> oldNames, Dictionary<string, string> newNames, bool isBoy)
{
    var stopwatch = Stopwatch.StartNew();
    var result = new List<NameInfo>();
    foreach (var name in oldNames)
    {
        result.Add(new NameInfo { 
            Name = name.Key, 
            Antal = int.Parse(name.Value.Replace(" ", "")),
            IsBoy = isBoy
        });
    }
    foreach (var name in newNames)
    {
        var ExistingName = result.FirstOrDefault(n => n.Name.ToLower() == name.Key.ToLower());
        if (ExistingName == null) result.Add(new NameInfo { 
            Name = name.Key, 
            Antal = int.Parse(name.Value.Replace(" ", "")), 
            IsBoy = isBoy 
        });
        else ExistingName.Antal += int.Parse(name.Value.Replace(" ", ""));
    }
    stopwatch.Stop();
    Console.WriteLine($"MergeNames: {stopwatch.ElapsedMilliseconds}ms");
    return result.OrderBy(n => n.Name).ToList();
}
static List<NameInfo> MergeNames(Dictionary<string, string> oldNames, Dictionary<string, string> newNames, bool isBoy)
{
    var stopwatch = Stopwatch.StartNew();
    var result = new Dictionary<string, NameInfo>(StringComparer.OrdinalIgnoreCase);

    void AddOrUpdate(Dictionary<string, string> names)
    {
        foreach (var pair in names)
        {
            var name = pair.Key;
            var antal = int.Parse(pair.Value.Replace(" ", ""));

            if (result.TryGetValue(name, out var existing))
            {
                existing.Antal += antal;
            }
            else
            {
                result[name] = new NameInfo
                {
                    Name = name,
                    Antal = antal,
                    IsBoy = isBoy
                };
            }
        }
    }

    AddOrUpdate(oldNames);
    AddOrUpdate(newNames);
    stopwatch.Stop();
    Console.WriteLine($"MergeNamesNew: {stopwatch.ElapsedMilliseconds}ms");
    return result.Values.OrderBy(n => n.Name).ToList();
}


