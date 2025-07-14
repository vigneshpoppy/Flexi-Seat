using FlexiSeat.Data;
using FlexiSeat.DbContext;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FlexiSeat.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class EmployeesController : ControllerBase
  {
    private readonly FlexiSeatDbContext _db;

    public EmployeesController(FlexiSeatDbContext db) => _db = db;

    [HttpGet("{adid}")]
    public async Task<ActionResult<Employee>> Get(string adid)
        => await _db.Employees.FindAsync(adid) is { } emp ? Ok(emp) : NotFound();

    [HttpPost]
    public async Task<IActionResult> Create(Employee dto)
    {
      _db.Employees.Add(dto);
      await _db.SaveChangesAsync();
      return CreatedAtAction(nameof(Get), new { adid = dto.EmployeeADID }, dto);
    }

    [HttpPut("{adid}")]
    public async Task<IActionResult> Update(string adid, Employee dto)
    {
      if (adid != dto.EmployeeADID) return BadRequest();
      _db.Entry(dto).State = EntityState.Modified;
      await _db.SaveChangesAsync();
      return NoContent();
    }

    [HttpDelete("{adid}")]
    public async Task<IActionResult> Delete(string adid)
    {
      var emp = await _db.Employees.FindAsync(adid);
      if (emp is null) return NotFound();
      _db.Employees.Remove(emp);
      await _db.SaveChangesAsync();
      return NoContent();
    }
  }
}
