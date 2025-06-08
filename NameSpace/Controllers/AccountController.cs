using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NameSpace.Dtos;
using NameSpace.Models;
using NameSpace.Services;
using System.Web;


namespace NameSpace.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly TokenProvider _tokenProvider;
        private readonly EmailService _emailProvider;
        public AccountController(UserManager<User> userManager, TokenProvider tokenProvider, EmailService emailProvider, AppDbContext context)
        {
            _userManager = userManager;
            _tokenProvider = tokenProvider;
            _emailProvider = emailProvider;
            _context = context;
        }
        // Registrering
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest registerUserRequest)
        {
            //add email confirmation
            var user = new User 
            { 
                UserName = registerUserRequest.Username, 
                Email = registerUserRequest.Email,
                LockoutEnabled = true,
                LockoutEnd = DateTimeOffset.MaxValue
            };

            var result = await _userManager.CreateAsync(user, registerUserRequest.Password);
            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = HttpUtility.UrlEncode(token);
                var email = _emailProvider.TemplateEmailVerification(user, encodedToken);
                if(email == null)
                {
                    return StatusCode(500, "Något gick fel med att skicka email");
                }
                await _emailProvider.SendEmailAsync(email.SendTo, email.Subject, email.Body);
                return Ok(user);
            }

            return BadRequest(result.Errors);
        }
        [HttpPost("verify-email")]
        public async Task<IActionResult> VerifyEmail([FromBody] ConfirmEmailDto emailDto)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(emailDto.Email);
                if(user == null)
                {
                    return NotFound("Could not find a pending email confirmation");
                }
                string decodedToken = HttpUtility.UrlDecode(emailDto.Token);
                var result = await _userManager.ConfirmEmailAsync(user, emailDto.Token);
                if (!result.Succeeded)
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var encodedToken = HttpUtility.UrlEncode(token);
                    var email = _emailProvider.TemplateEmailVerification(user, encodedToken);
                    if (email == null)
                    {
                        return StatusCode(500, "Något gick fel med att skicka email");
                    }
                    await _emailProvider.SendEmailAsync(email.SendTo, email.Subject, email.Body);
                    return BadRequest("Ogiltig token!");
                }
                user.LockoutEnd = null; // Gittar låset
                await _userManager.UpdateAsync(user);
                return Ok(user);

            } catch (Exception ex)
            {
                if (ex is InvalidOperationException)
                {
                    return BadRequest($"En användare med email: {emailDto.Email} finns redan.");
                }
                else
                {
                    return BadRequest(ex.Message);
                }
            }

        }
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Ta bort JWT-token från cookie (eller session)
            Response.Cookies.Delete("auth_token");

            return Ok(new { message = "User logged out" });
        }
        // Inloggning
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username) ?? await _userManager.FindByEmailAsync(request.Username);
            if (user == null) return Unauthorized("No such user" );
            if (user.LockoutEnd != null && user.LockoutEnd > DateTimeOffset.UtcNow)
            {
                return Unauthorized("Email is not verified");
            }

            var checkPassword = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!checkPassword)
            {
                return Unauthorized("Wrong password");
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
           // await _emailProvider.SendEmailAsync("haglund_dragon@hotmail.com", "test", "this is a test email");
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
        [Authorize]
        [HttpPost("request-partner")]
        public async Task<IActionResult> RequestPartner([FromBody] string partner)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized("Not logged in");
            }
            var partnerToAdd = await _userManager.FindByEmailAsync(partner) ?? await _userManager.FindByNameAsync(partner);
            if (partnerToAdd == null)
            {
                return NotFound("Could not find a partner with that name or email");
            }
            var newPartner = new PartnerRequest
            {
                Id = Guid.NewGuid(),
                RequestingUserId = currentUser.Id,
                RecivingUserId = partnerToAdd.Id
            };

            _context.PartnerRequests.Add(newPartner);
            await _context.SaveChangesAsync();

            return Ok();
        }
        [HttpPost("add-partner")]
        public async Task<IActionResult> ConfirmPartner([FromQuery] Guid partnerRequestId)
        {
            var partnerRequest = _context.PartnerRequests.FirstOrDefault(p => p.Id == partnerRequestId);

            if (partnerRequest == null)
            {
                return BadRequest("No partner request with that token was found");
            }

            var requestingUser = await _userManager.FindByIdAsync(partnerRequest.RequestingUserId);
            var recivingUser = await _userManager.FindByIdAsync(partnerRequest.RecivingUserId);

            if (requestingUser == null || recivingUser == null)
            {
                return BadRequest("user not found");
            }
            requestingUser.PartnerUserId = recivingUser.Id;
            recivingUser.PartnerUserId = requestingUser.Id;

            await _userManager.UpdateAsync(requestingUser);
            await _userManager.UpdateAsync(recivingUser);

            _context.PartnerRequests.Remove(partnerRequest);
            _context.SaveChanges();

            return Ok();
        }
        [Authorize]
        [HttpGet("partner-requests")]
        public async Task<IActionResult> GetPartnerRequests()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }
            
            var myPartner = _context.Users.FirstOrDefault(u => u.Id == currentUser.PartnerUserId);

            var MyPartnerRequests = _context.PartnerRequests.Where(p => p.RequestingUserId == currentUser.Id)
                .Select(u => new PartnerRequestSimple 
                { 
                    Id = u.Id,
                    UserId = u.RecivingUserId
                })
                .ToList();

            var PartnerRequests = _context.PartnerRequests.Where(p => p.RecivingUserId == currentUser.Id)
                .Select(u => new PartnerRequestSimple
            {
                Id = u.Id,
                UserId = u.RequestingUserId
                })
                .ToList();
            var result = new PartnerRequestsDto
            {
                CurrentPartner = myPartner?.UserName,
                MyPartnerRequests = MyPartnerRequests,
                PartnerRequests = PartnerRequests,

            };
            return Ok(result);
        }
        [Authorize]
        [HttpDelete("partner-request")]
        public void DeletePartnerRequest([FromQuery] Guid requestId)
        {
            _context.PartnerRequests.Remove(
                _context.PartnerRequests.First(p => p.Id == requestId)
            );
            _context.SaveChangesAsync();
        }
    }
}

