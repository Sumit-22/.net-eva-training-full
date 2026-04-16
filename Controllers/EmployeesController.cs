using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 
using MyFirstApi.Data;
using MyFirstApi.Models;
namespace MyFirstApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
  //  [Authorize(AuthenticationSchemes = "BasicAuth")]
     [Authorize] // JWT auth
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public EmployeesController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Get()
        {   var emp = _context.Employees.ToList();
            return Ok(emp);
        }
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var emp = _context.Employees.Find(id);
            if(emp == null)
            {
                return NotFound("Employee not found");

            }
            return Ok(emp);

        }

        // post full update
        [HttpPost]
        //[HttpPost("create-employee")]
        public IActionResult AddEmployee(Employee employee)
        {
            _context.Employees.Add(employee);
            _context.SaveChanges();
            return Ok("Employee added successfully");
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public IActionResult DeleteEmployee(int id)
        {
            var employee = _context.Employees.Find(id);
            if (employee == null)
                return NotFound("Employee not found");
            _context.Employees.Remove(employee);
            _context.SaveChanges();
            return Ok("Employee deleted successfully");
        }

       [HttpGet("search")]
    public IActionResult Search(
         [FromQuery] string? name,
         [FromQuery] string? department,
         [FromQuery] decimal? minSalary,
         [FromQuery] decimal? maxSalary)
    {
         var query = _context.Employees.AsQueryable();

        if (!string.IsNullOrEmpty(name))
              query = query.Where(e => e.FirstName.Contains(name));

        if (!string.IsNullOrEmpty(department))
              query = query.Where(e => e.Department == department);

        if (minSalary.HasValue)
              query = query.Where(e => e.Salary >= minSalary);

        if (maxSalary.HasValue)
            query = query.Where(e => e.Salary <= maxSalary);

        var result = query.ToList();

        return Ok(result);
      }
    }
}