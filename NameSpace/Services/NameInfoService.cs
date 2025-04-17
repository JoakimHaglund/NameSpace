using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NameSpace.Dtos;
using NameSpace.Models;
using System.Security.Claims;

namespace NameSpace.Services
{
    public class NameInfoService
    {
        private readonly AppDbContext _context;
        private readonly FileReader _fileReader;
        public NameInfoService(AppDbContext context, FileReader fileReader) 
        { 
            _context = context;
            _fileReader = fileReader;
        }
        public async Task<List<NameInfo>?> GetUnreactedByLetter(string userId, char letter, int pageNumber = 1, int pageSize = 50)
        {
            // Validera att pageSize är mellan 50 och 100, om inte ge en default på 50
            if (pageSize < 50 || pageSize > 100)
            {
                pageSize = 50; // Kan du ändra till 100 om du tycker det
            }
            var user = await _context.Users.Include(u => u.UserReactions).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            var reactedTo = user.UserReactions?.Select(n => n.NameInfoId).ToList() ?? new List<Guid>();
            // Sätt startpunkt (Skip) och antalet objekt att hämta (Take)
            var nameInfo = await _context.NameInfos
                .Where(n => n.Name.StartsWith(letter.ToString().ToUpper()) && !reactedTo.Contains(n.Id))
                .Skip((pageNumber - 1) * pageSize) // Hoppa över föregående sidor
                .Take(pageSize) // Hämta så många som specificerats
                .ToListAsync();

            return nameInfo;
        }
        public async Task<(int Added, int Updated)?> HandleFileUpload(IFormFile file)
        {
            if (file == null || file.Length == 0) return null;

            var namesDtoSet = await _fileReader.ReadCSV(file);

            if (namesDtoSet == null) return null;

            var names = namesDtoSet.Select(x => x.Name).ToList();
            var dbNameInfos = await _context.NameInfos.Where(n => names.Contains(n.Name)).ToListAsync();

            var dbDictNameInfos = dbNameInfos.ToDictionary(n => n.Name, StringComparer.OrdinalIgnoreCase);

            var nameInfosToUpdate = new List<NameInfo>();
            var nameInfosToAdd = new List<NameInfo>();

            foreach (var namesDto in namesDtoSet)
            {
                if (dbDictNameInfos.TryGetValue(namesDto.Name, out var existingNameInfo))
                {
                    // 🚨 UPDATE
                    existingNameInfo.Antal += namesDto.Antal;
                    existingNameInfo.DescriptionOfName = namesDto.DescriptionOfName ?? existingNameInfo.DescriptionOfName;
                    existingNameInfo.Gender = (Gender)namesDto.Gender;

                    nameInfosToUpdate.Add(existingNameInfo);
                }
                else
                {
                    nameInfosToAdd.Add(new NameInfo
                    {
                        Id = new Guid(),
                        Name = namesDto.Name,
                        Antal = namesDto.Antal,
                        DescriptionOfName = namesDto.DescriptionOfName,
                        Gender = (Gender)namesDto.Gender
                    });
                }
            }

            if (nameInfosToAdd.Any())
            {
                await _context.AddRangeAsync(nameInfosToAdd);
            }
            if (nameInfosToUpdate.Any())
            {
                _context.UpdateRange(nameInfosToUpdate);
            }

            await _context.SaveChangesAsync();
            return (Added: nameInfosToAdd.Count, Updated: nameInfosToUpdate.Count);

        }
        public async Task<Response<NameInfo>> UpdateNameInfo(NameInfo nameInfo)
        {
            var existingNameInfo = await _context.NameInfos.FindAsync(nameInfo.Id);
            if (existingNameInfo == null) return new Response<NameInfo> 
            { 
                Success = false, 
                Message = "Not found in database", 
                data = null 
            };
            existingNameInfo.Name = nameInfo.Name;
            existingNameInfo.DescriptionOfName = nameInfo.DescriptionOfName;
            existingNameInfo.Antal = nameInfo.Antal;
            existingNameInfo.Gender = nameInfo.Gender;

            _context.NameInfos.Update(existingNameInfo);
            await _context.SaveChangesAsync();

            return new Response<NameInfo> 
            {
                Success = true,
                Message = $"Successfully updated {existingNameInfo.Id}",
                data = existingNameInfo
            };
        }
    }
}
