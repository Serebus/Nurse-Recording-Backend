using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nurse_Recording_Backend.Data;
using Nurse_Recording_Backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace Nurse_Recording_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public AppointmentsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments()
    {
        return await _context.Appointments.Include(a => a.Patient).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetAppointment(int id)
    {
        var appointment = await _context.Appointments.Include(a => a.Patient).FirstOrDefaultAsync(a => a.Id == id);
        if (appointment == null) return NotFound();
        return appointment;
    }

    [HttpPost]
    public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
    {
        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
    {
        if (id != appointment.Id) return BadRequest();
        _context.Entry(appointment).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!AppointmentExists(id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointment(int id)
    {
        var appointment = await _context.Appointments.FindAsync(id);
        if (appointment == null) return NotFound();
        _context.Appointments.Remove(appointment);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private bool AppointmentExists(int id) => _context.Appointments.Any(e => e.Id == id);
}
