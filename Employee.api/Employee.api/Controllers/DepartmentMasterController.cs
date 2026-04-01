using Employee.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentMasterController : ControllerBase
    {
        private readonly EmployeeDbContext _context;

        public DepartmentMasterController(EmployeeDbContext context)
        {
            _context = context;
        }
        [HttpGet("GetAllDepartments")]
        public IActionResult GetDepartment()
        {
            var depList = _context.Departments.ToList();
            return Ok(depList);
        }

        [HttpPost("AddDepartment")]
        public IActionResult AddDepartment([FromBody] Department dept)
        {
            bool exists = _context.Departments
                .Any(d => d.departmentName.ToLower() == dept.departmentName.ToLower());

            if (exists)
            {
                return BadRequest("Department name must be unique.");
            }

            _context.Departments.Add(dept);
            _context.SaveChanges();
            return Created("Department Added Successfully", dept);
        }

        [HttpPut("UpdateDepartment")]
        public IActionResult UpdateDepartment([FromBody] Department dept)
        {
            var existingDept = _context.Departments.Find(dept.departmentId);
            if (existingDept == null)
            {
                return NotFound("Department not found");
            }

            existingDept.departmentName = dept.departmentName;
            existingDept.isActive = dept.isActive;
            _context.SaveChanges();
            return Created("Department Updated Successfully", dept);
        }

        [HttpDelete("DeleteDepartment/{id}")]
        public IActionResult DeleteDepartment(int id)
        {
            var dept = _context.Departments.Find(id);
            if (dept == null)
            {
                return NotFound("Department not found");
            }
            _context.Departments.Remove(dept);
            _context.SaveChanges();
            return Created("Department Deleted Successfully", dept);
        }
    }   
}
