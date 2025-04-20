using Microsoft.AspNetCore.Authorization;
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

        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Ta bort JWT-token från cookie (eller session)
            Response.Cookies.Delete("auth_token");  // Byt namn till vad du kallar din cookie

            return Ok(new { message = "User logged out" });
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

            // Skicka tillbaka JWT som HttpOnly cookie
            Response.Cookies.Append("auth_token", token, new CookieOptions
            {
                HttpOnly = true,       // Skyddar mot XSS
                Secure = false,         // Se till att den bara skickas över HTTPS
                SameSite = SameSiteMode.Lax, // Skyddar mot CSRF
                Expires = DateTime.UtcNow.AddDays(1) // Expiration time
            });

            return Ok(new { message = "Logged in successfully" });
        }
        [HttpGet("check-login")]
        public IActionResult CheckLoginStatus()
        {
            var token = Request.Cookies["auth_token"];
            foreach (var cookie in Request.Cookies)
            {
                Console.WriteLine($"COOKIE: {cookie.Key} = {cookie.Value}");
            }

            if (token == null)
            {
                return Unauthorized("No token found");
            }

            var claims = _tokenProvider.ValidateJwtToken(token); // Verifiera JWT-token
            if (claims == null)
            {
                return Unauthorized("Invalid token");
            }

            return Ok(new { message = "User is logged in" });
        }
    }
}

