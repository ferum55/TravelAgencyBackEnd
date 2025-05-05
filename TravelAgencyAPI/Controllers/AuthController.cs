using Microsoft.AspNetCore.Mvc;
using TravelAgencyAPI;
using TravelAgencyAPI.DTO;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using TravelAgencyAPI.Models;

namespace TravelAgencyAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly TravelAgencyContext _context;

        public AuthController(TravelAgencyContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            try
            {
                var user = _context.Users
                    .Include(u=>u.Role)
                    .SingleOrDefault(u=>u.UserName==request.UserName);
                if (user == null) return Unauthorized("Invalid Username!");

                var sha256 = SHA256.Create();
                var hashed = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password)));

                if (hashed != user.PasswordHash)
                    return Unauthorized("Invalid password");

                return Ok(new
                {
                    username = user.UserName,
                    role = user.Role.RoleName
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"💥 Server error: {ex.Message}");
            }
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.UserName == request.Username))
                return BadRequest("Username already exists");
            var role = await _context.UserRoles.SingleOrDefaultAsync(r => r.RoleName == request.RoleName);
            if (role==null) return BadRequest("Bad request");
            var sha256 = SHA256.Create();
            var hashed = Convert.ToBase64String(sha256.ComputeHash(Encoding.UTF8.GetBytes(request.Password)));

            var user = new User
            {
                UserName = request.Username,
                PasswordHash = hashed,
                RoleId = role.RoleId
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return Ok(new { message = "User registered successfully" });
        }
    }
}
