using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using MyFirstApi.Data;
using MyFirstApi.Models;

namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
         private readonly AppDbContext _context;

        private readonly string key = "THIS_IS_MY_SUPER_SECRET_KEY_123456789012345";

        public AuthController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // REGISTER (USER + EMPLOYEE)
    [HttpPost("register")]
    public async Task<IActionResult> Register(LoginModel model)
    {
        var userExists = await _userManager.FindByNameAsync(model.Username);
        if (userExists != null)
            return BadRequest("User already exists");

        var user = new IdentityUser
        {
            UserName = model.Username,
            Email = model.Username + "@evaWinOptimize.com"
        };

        var result = await _userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
            return BadRequest(result.Errors);

        // ROLE
        if (!await _roleManager.RoleExistsAsync("User"))
            await _roleManager.CreateAsync(new IdentityRole("User"));

        await _userManager.AddToRoleAsync(user, "User");

        // CREATE EMPLOYEE
        var emp = new Employee
        {
            FirstName = model.Username,
            LastName = "User",
            Email = user.Email,
            Department = "General",
            Salary = 0,
            UserId = user.Id
        };

        _context.Employees.Add(emp);
        await _context.SaveChangesAsync();

        return Ok("User + Employee created");
    }
    
    
     // LOGIN
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);

        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach (var role in roles)
                claims.Add(new Claim(ClaimTypes.Role, role));

            var token = new JwtSecurityToken(
                expires: DateTime.Now.AddHours(3),
                claims: claims,
                signingCredentials: new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                    SecurityAlgorithms.HmacSha256)
            );

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token)
            });
        }

        return Unauthorized();
    }


    //  MAKE ADMIN API
     [HttpPost("make-admin/{username}")]
    public async Task<IActionResult> MakeAdmin(string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound("User not found");

        if (!await _roleManager.RoleExistsAsync("Admin"))
            await _roleManager.CreateAsync(new IdentityRole("Admin"));

        await _userManager.AddToRoleAsync(user, "Admin");

        return Ok($"{username} is now Admin");
    }
        // current user
        [HttpGet("me")]
        [Authorize]
        public IActionResult Me()
        {
            return Ok(new
            {
                user = User.Identity.Name
            });
        }
    }
}