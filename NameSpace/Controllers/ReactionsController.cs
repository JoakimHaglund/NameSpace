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
        public async Task<IActionResult> GetReactions([FromQuery] string reaction)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("This user does not exist");

            var reactions = await _context.UserReactions
                .Include(r => r.NameInfo)
                .Where(r => r.UserId == userId)
                .ToListAsync();
            // Om ingen matchning finns, returnera NotFound istället för null
            if (!reactions.Any()) return BadRequest($"Inget namn hittades som börjar med ''");
            if (reaction == "favorites")
            {
                reactions = reactions.Where(r => r.IsAFavorite == true).ToList();
            }
            else if (reaction == "liked")
            {
                reactions = reactions.Where(r => r.Reaction == ReactionType.Like).ToList();

            }
            else if (reaction == "disliked")
            {
                reactions = reactions.Where(r => r.Reaction == ReactionType.Dislike).ToList();
            }

            return Ok(reactions);
        }

        // POST api/<ValuesController>
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
                    IsAFavorite = reaction.IsAFavorite
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




        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}

