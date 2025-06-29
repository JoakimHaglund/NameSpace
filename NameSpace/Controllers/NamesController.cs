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
    public class NamesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly FileReader _fileReader;
        private readonly NameInfoService _nameInfoService;
        public NamesController(AppDbContext context, NameInfoService nameInfoService) 
        {
            _context = context;
            _fileReader = new FileReader();
            _nameInfoService = nameInfoService;
        }


        // GET api/<ValuesController>/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var nameInfo = await _context.NameInfos.FindAsync(id);
            if (nameInfo == null) return NotFound(id);
            return Ok(nameInfo);
        }
        [HttpGet("by-letter")]
        public async Task<IActionResult> GetByFirstChar([FromQuery] NameQueryDto nameQueryDto)
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null) return Unauthorized("This user does not exist");

            var nameInfo = await _nameInfoService.GetUnreactedByLetter(nameQueryDto, userId);
            if (nameInfo == null) return Unauthorized("User not found");

            // Om ingen matchning finns, returnera NotFound istället för null
            if (!nameInfo.Any()) return NotFound($"Inget namn hittades som börjar med '{nameQueryDto.Letter}'");

            return Ok(nameInfo);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        // POST api/<ValuesController>
        [Authorize(Roles = "Admin")]
        [HttpPost("upload-file")]
        public async Task<IActionResult> FileUpload(IFormFile file)
        {
            var result = await _nameInfoService.HandleFileUpload(file);

            if (result == null) return BadRequest("Empty or malformed file");

            var (added, updated) = result.Value;

            return Ok(new { added, updated });

        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(Guid id, [FromBody]NameInfo nameInfo)
        {
            if (id != nameInfo.Id) return BadRequest("Id mismatch");

            var result = await _nameInfoService.UpdateNameInfo(nameInfo);

            if (!result.Success) return BadRequest(result.Message);

            return Ok(result);
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
