using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly string key = "THIS_IS_MY_SUPER_SECRET_KEY_123456789012345";

        [HttpPost("login")]
        [AllowAnonymous] // 🔥 login open hona chahiye
        public IActionResult Login([FromBody] LoginModel model)
        {
            //  null safety
            if (model == null)
                return BadRequest("Invalid request");

            // simple hardcoded auth (demo purpose)
            if (model.Username == "admin" && model.Password == "123")
            {
                var claims = new[]
                {
                    new Claim(ClaimTypes.Name, model.Username),
                    new Claim(ClaimTypes.Role, "Admin") // 🔥 future role use
                };

                var keyBytes = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(key)
                );

                var creds = new SigningCredentials(
                    keyBytes,
                    SecurityAlgorithms.HmacSha256
                );

                var token = new JwtSecurityToken(
                    claims: claims,
                    expires: DateTime.Now.AddHours(1),
                    signingCredentials: creds
                );

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new
                {
                    token = tokenString,
                    message = "Login successful"
                });
            }

            return Unauthorized("Invalid username or password");
        }
    }
}