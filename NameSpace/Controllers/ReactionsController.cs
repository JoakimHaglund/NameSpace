using CsvHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NameSpace.Dtos;
using NameSpace.Models;
using NameSpace.Services;
using System;
using System.Formats.Asn1;
using System.Globalization;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace NameSpace.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReactionsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReactionsController(AppDbContext context)
        {
            _context = context;

        }
        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok();
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var nameInfo = await _context.NameInfos.FindAsync(id);
            if (nameInfo == null) return NotFound(id);
            return Ok(nameInfo);
        }
        [HttpGet("reactions")]
        public async Task<IActionResult> GetReactions([FromQuery] ReactionType reaction)
        {

            var user = await _context.Users
                .Include(u => u.PartnerUser)
                .ThenInclude(p => p.UserReactions)
                .FirstOrDefaultAsync(u => u.Id == User
                    .FindFirstValue(ClaimTypes.NameIdentifier)
                    );

            if (user == null) return Unauthorized("User not found");

            var reactions = await _context.UserReactions
                .Include(r => r.NameInfo)
                .Where(r => r.UserId == user.Id)
                .ToListAsync();

            var partnerReactionDict = user.PartnerUser?.UserReactions?
                .ToDictionary(r => r.NameInfoId, elementSelector: r => r.Reaction) ?? [];

            if (!reactions.Any()) return NotFound($"Inga reaktioner hittades");

            switch (reaction)
            {
                case ReactionType.Favorite:
                    reactions = reactions.Where(r => r.Reaction == ReactionType.Favorite).ToList();
                    break;
                case ReactionType.Like:
                    reactions = reactions.Where(r => r.Reaction == ReactionType.Like).ToList();
                    break;
                case ReactionType.Dislike:
                    reactions = reactions.Where(r => r.Reaction == ReactionType.Dislike).ToList();
                    break;
            }

            var result = reactions.Select(n => new ReactionListDto
            {
                NameInfoId = n.NameInfoId,
                Name = n.NameInfo!.Name,
                Count = n.NameInfo.Count,
                Gender = (int)n.NameInfo.Gender,
                PartnerReaction = partnerReactionDict.ContainsKey(n.NameInfo.Id) ? (int)partnerReactionDict[n.NameInfo.Id] : null
            }).ToList();
    

            return Ok(result);
        }

        [HttpPost("reactions")]
        public async Task<IActionResult> PostReactions([FromBody] List<ReactionDto> reactions)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("User not found");

            var reactionsToAdd = new List<UserReaction>();
            foreach (var reaction in reactions)
            {
                reactionsToAdd.Add(new UserReaction
                {
                    UserId = userId,
                    NameInfoId = reaction.NameInfoId,
                    Reaction = (ReactionType)reaction.Reaction,
                });
            }
            try
            {
                _context.AddRange(reactionsToAdd);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("Whalla det small: " + ex.Message);
            }
            return Ok("reactions added");
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateReactions([FromBody] List<ReactionDto> reactionsToUpdate)
        {
            if (reactionsToUpdate == null || !reactionsToUpdate.Any())
            {
                return BadRequest("Inga ändringar hittades");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized("User not found");
            
            var ids = reactionsToUpdate.Select(n => n.NameInfoId).ToList();
            var reactions = await _context.UserReactions
                .Where(n => n.UserId == userId && ids
                    .Contains(n.NameInfoId))
                .ToListAsync();

            foreach (var reaction in reactions) 
            { 
                var updated = reactionsToUpdate.FirstOrDefault(n => n.NameInfoId == reaction.NameInfoId);
                if (updated == null) continue;

                if (!Enum.IsDefined(typeof(ReactionType), updated.Reaction)) continue;
                reaction.Reaction = (ReactionType)updated.Reaction;
            }
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRange([FromBody] List<ReactionDto> reactionsToDelete)
        {
            if (reactionsToDelete == null || !reactionsToDelete.Any())
            {
                return BadRequest("Inga ändringar hittades");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Unauthorized("User not found");

            var ids = reactionsToDelete.Select(n => n.NameInfoId).ToList();
            var reactions = await _context.UserReactions
                .Where(n => n.UserId == userId && ids
                    .Contains(n.NameInfoId))
                .ToListAsync();
            if (!reactions.Any()) return NotFound("Inga reaktioner hittades att ta bort");

            _context.RemoveRange(reactions);
            _context.SaveChanges();
            return Ok();
        }
    }
}

