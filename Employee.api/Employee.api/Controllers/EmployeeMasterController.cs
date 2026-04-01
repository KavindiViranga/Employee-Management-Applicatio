using Employee.api.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Employee.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeMasterController : ControllerBase
    {       
        private readonly EmployeeDbContext _context;

        public EmployeeMasterController(EmployeeDbContext context)
        {
            _context = context;
        }

        //GET (All Employees)
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var data = await _context.Employees.ToListAsync();
                return Ok(data);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // GET BY ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                    return NotFound(new { Message = "Employee not found" });

                return Ok(employee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // CREATE
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] EmployeeModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Unique validation
                bool exists = await _context.Employees
                    .AnyAsync(e => e.contactNo == model.contactNo || e.email == model.email);

                if (exists)
                    return BadRequest(new { Message = "Contact No or Email already exists" });

                model.createdDate = DateTime.Now;
                model.modifiedDate = DateTime.Now;

                _context.Employees.Add(model);
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Employee created successfully", Data = model });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }          

        // UPDATE
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeModel model)
        {
            try
            {
                if (id != model.employeeId)
                    return BadRequest("ID mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var existing = await _context.Employees.FindAsync(id);

                if (existing == null)
                    return NotFound("Employee not found");

                // Unique validation
                bool exsits = await _context.Employees
                    .AnyAsync(e =>
                    (e.contactNo == model.contactNo || e.email == model.email)
                    && e.employeeId != id);

                if (exsits)
                    return BadRequest("Contact No or Email already exists");


                // Update fields
                existing.name = model.name;
                existing.contactNo = model.contactNo;
                existing.email = model.email;
                existing.city = model.city;
                existing.state = model.state;
                existing.pincode = model.pincode;
                existing.altContactNo = model.altContactNo;
                existing.address = model.address;
                existing.designationId = model.designationId;
                existing.modifiedDate = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new { Message = "Employee updated successfully" ,Data = existing });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);

                if (employee == null)
                    return NotFound("Employee not found");

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return Ok("Employee Deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        // ADVANCED GET: FILTER + SORT + PAGINATION
        [HttpGet("filter")]
        public async Task<IActionResult> Filter(
            string? search,           
            int? designationId,
            string? sortBy = "name",
            string? sortDir = "asc",
            int pageNumber = 1,
            int pageSize = 10)
        {
            try
            {
                var query = _context.Employees.AsQueryable();

                // searching
                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(e =>
                    e.name.Contains(search) ||
                    e.contactNo.Contains(search) ||
                    e.email.Contains(search) ||
                    e.city.Contains(search));
                }

                // filtering
                if (designationId.HasValue)
                {
                    query = query.Where(e => e.designationId == designationId);
                }

                // Sorting
                switch (sortBy?.ToLower())
                {
                    case "name":
                        query = sortDir == "desc"
                            ? query.OrderByDescending(e => e.name)
                            :query.OrderBy(e => e.name);
                        break;

                    case "createddate":
                        query = sortDir == "desc"
                            ? query.OrderByDescending(e => e.createdDate)
                            : query.OrderBy(e => e.createdDate);
                        break;

                    default:
                        query = query.OrderBy(e => e.employeeId);
                        break;
                }

                // Pagination
                int totalRecords = await query.CountAsync();

                var data = await query
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                return Ok(new
                {
                    TotalRecords = totalRecords,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    Data = data
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var user = await _context.Employees
                    .FirstOrDefaultAsync(e => e.email == model.email && e.contactNo == model.contactNo);

                if (user == null)
                    return Unauthorized(new { Message = "Invalid Credentials" });

                return Ok(new
                {
                    message = "Login successful",
                    data = new
                    {
                        user.employeeId,
                        user.name,
                        user.email,
                        user.contactNo,
                        user.designationId,
                        user.role
                    }
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
    }
}
