using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nurse_Recording_Backend.Data;
using Nurse_Recording_Backend.Models;

namespace Nurse_Recording_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Nurse,Admin")]
public class FollowupsController : ControllerBase
{
    private readonly AppDbContext _context;

    public FollowupsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Followup>>> GetFollowups()
    {
        return await _context.Followups.Include(f => f.Patient).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Followup>> GetFollowup(int id)
    {
        var followup = await _context.Followups
            .Include(f => f.Patient)
            .FirstOrDefaultAsync(f => f.Id == id);

        if (followup == null) return NotFound();
        return followup;
    }

    [HttpPost]
    public async Task<ActionResult<Followup>> PostFollowup(Followup followup)
    {
        var record = await _context.PatientRecords.FindAsync(followup.RecordId);
        if (record != null && record.Status == "Closed")
        {
            return BadRequest("Cannot add followup to a closed patient record.");
        }

        if (followup.Patient != null)
        {
            if (followup.PatientId == 0) followup.PatientId = followup.Patient.Id;
            followup.Patient = null!;
        }

        _context.Followups.Add(followup);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetFollowup), new { id = followup.Id }, followup);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutFollowup(int id, Followup followup)
    {
        if (id != followup.Id) return BadRequest();

        if (followup.Patient != null)
        {
            if (followup.PatientId == 0) followup.PatientId = followup.Patient.Id;
            followup.Patient = null!;
        }

        _context.Entry(followup).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!FollowupExists(id)) return NotFound();
            throw;
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteFollowup(int id)
    {
        var followup = await _context.Followups.FindAsync(id);
        if (followup == null) return NotFound();

        _context.Followups.Remove(followup);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool FollowupExists(int id) => _context.Followups.Any(e => e.Id == id);
}
