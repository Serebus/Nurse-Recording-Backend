using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nurse_Recording_Backend.Data;
using Nurse_Recording_Backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace Nurse_Recording_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Nurse,Admin")]
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
        var appointments = await _context.Appointments.Include(a => a.Patient).ToListAsync();
        foreach (var appt in appointments)
        {
            if (IsPastAppointment(appt))
            {
                appt.Status = "Closed";
                _context.Entry(appt).State = EntityState.Modified;
            }
        }
        await _context.SaveChangesAsync();
        return appointments;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Appointment>> GetAppointment(int id)
    {
        var appointment = await _context.Appointments.Include(a => a.Patient).FirstOrDefaultAsync(a => a.Id == id);
        if (appointment == null) return NotFound();

        if (IsPastAppointment(appointment) && appointment.Status != "Closed")
        {
            appointment.Status = "Closed";
            _context.Entry(appointment).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        return appointment;
    }

    [HttpPost]
    public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
    {
        // Validate date/time not in past
        if (!DateTime.TryParse($"{appointment.Date:yyyy-MM-dd} {appointment.Time}", out var apptDateTime))
        {
            return BadRequest("Invalid Date or Time format.");
        }
        
        if (apptDateTime < DateTime.Now)
        {
            return BadRequest("Cannot schedule appointment in the past.");
        }
        appointment.Status = "Pending";

        // Prevent EF Core from attempting to insert a new Patient if one is passed in the payload
        if (appointment.Patient != null)
        {
            if (appointment.PatientId == 0) appointment.PatientId = appointment.Patient.Id;
            appointment.Patient = null;
        }

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
    {
        if (id != appointment.Id) return BadRequest();
        // Validate date/time not in past
        if (!DateTime.TryParse($"{appointment.Date:yyyy-MM-dd} {appointment.Time}", out var apptDateTime))
        {
            return BadRequest("Invalid Date or Time format.");
        }
        
        if (apptDateTime < DateTime.Now)
        {
            return BadRequest("Cannot schedule appointment in the past.");
        }
        appointment.Status = "Pending";

        // Prevent EF Core from attempting to insert/update the Patient navigation property
        if (appointment.Patient != null)
        {
            if (appointment.PatientId == 0) appointment.PatientId = appointment.Patient.Id;
            appointment.Patient = null;
        }

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

    private bool IsPastAppointment(Appointment appt)
    {
        // Don't close appointments just because of a bad format
        if (!DateTime.TryParse($"{appt.Date:yyyy-MM-dd} {appt.Time}", out var apptDateTime))
            return false; 
            
        // Add a 5-minute grace period
        return apptDateTime.AddMinutes(5) < DateTime.Now;
    }

    private bool AppointmentExists(int id) => _context.Appointments.Any(e => e.Id == id);
}
