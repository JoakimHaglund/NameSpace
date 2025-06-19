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
                Antal = n.NameInfo.Antal,
                Gender = (int)n.NameInfo.Gender,
                PartnerReaction = partnerReactionDict.ContainsKey(n.NameInfo.Id) ? (int)partnerReactionDict[n.NameInfo.Id] : null
            }).ToList();
    

            return Ok(result);
        }

        // POST api/<ValuesController>
        [HttpPost("reactions")]
        public async Task<IActionResult> PostReactions([FromBody] List<ReactionDto> reactions)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("User not found");

            var existingReactions = await _context.UserReactions
                .Where(u => u.UserId == userId)
                .Select(r => r.NameInfoId)
                .ToListAsync();

            var reactionsToAdd = reactions.Where(r => !existingReactions.Contains(r.NameInfoId))
                .Select(r => new UserReaction
                {
                    UserId = userId,
                    NameInfoId = r.NameInfoId,
                    Reaction = (ReactionType)r.Reaction
                }).ToList();
            if (!reactionsToAdd.Any())
            {
                return Ok("No reactions to add");
            }
            try
            {
                _context.AddRange(reactionsToAdd);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return BadRequest("Error while adding reactions: " + ex.Message);
            }
            return Ok("Reactions added");
        }




        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

