using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NameSpace.Models;
using NameSpace.Services;


namespace NameSpace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;

        private readonly TokenProvider _tokenProvider;
        public AccountController(UserManager<User> userManager, TokenProvider tokenProvider)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
        }

        // Registrering
        [HttpPost("register")]
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = new User { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                return Ok(user);
            }

            return BadRequest(result.Errors);
        }

        // Inloggning
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null) return Unauthorized();

            var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!checkPassword)
            {
                return Unauthorized();
            }

            var token = _tokenProvider.Create(user);

            return Ok(token);
        }
    }
}

