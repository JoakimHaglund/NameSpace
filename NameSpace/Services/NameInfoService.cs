using Microsoft.EntityFrameworkCore;
using NameSpace.Dtos;
using NameSpace.Models;


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
        public async Task<List<NameInfoDto>?> GetUnreactedByLetter(NameQueryDto nameQueryDto, string? userId)
        {
            if (nameQueryDto.PageSize < 1)
            {
                nameQueryDto.PageSize = 1;
            }

            var user = await _context.Users.Include(u => u.UserReactions).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return null;

            var reactedTo = user.UserReactions?.Select(n => n.NameInfoId).ToList() ?? new List<Guid>();

            var query = _context.NameInfos
                .Where(n => !reactedTo.Contains(n.Id));

            if(nameQueryDto.Letter == '?')
            {
                query = query.OrderBy(x => Guid.NewGuid());
            }
            else
            {
                query = query
                    .Where(n => n.Name.StartsWith(nameQueryDto.Letter.ToString().ToUpper()))
                    .OrderBy(n => n.Name);
            }

            query = FilterByNameCount(query, nameQueryDto);
            var nameInfo = GetPage(query, nameQueryDto.PageNumber, nameQueryDto.PageSize);

            if (nameQueryDto.Letter != '?')
            {
                nameInfo = nameInfo.OrderBy(n => n.Name);
            }
            var result = ConvertResultToDto(nameInfo, user);

            return result;
        }

        private IQueryable<NameInfo> FilterByNameCount(IQueryable<NameInfo> query, NameQueryDto nameQueryDto)
        {
            if (nameQueryDto.MinCount.HasValue)
            {
                query = query.Where(n => n.Antal >= nameQueryDto.MinCount);
            }
            if (nameQueryDto.MaxCount.HasValue)
            {
                query = query.Where(n => n.Antal >= nameQueryDto.MaxCount);
            }
            return query;
        }
        private IQueryable<NameInfo> GetPage(IQueryable<NameInfo> query, int pageNumber, int pageSize)
        {
            return query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);
        }
        private List<NameInfoDto> ConvertResultToDto(IQueryable<NameInfo> nameInfo, User user)
        {
            var partnerReactionDict = user.PartnerUser?.UserReactions?.ToDictionary(r => r.NameInfoId, elementSelector: r => r.Reaction)
                           ?? new Dictionary<Guid, ReactionType>();
            var result = nameInfo.Select(n => new NameInfoDto
            {
                NameInfoId = n.Id,
                Name = n.Name,
                Count = n.Antal,
                DescriptionOfName = n.DescriptionOfName,
                Gender = (int)n.Gender,
                PartnerReaction = partnerReactionDict.ContainsKey(n.Id) ? (int)partnerReactionDict[n.Id] : null
            }).ToList();

            return result;
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
