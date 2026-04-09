using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Nurse_Recording_Backend.Data;
using Nurse_Recording_Backend.Models;
using Microsoft.AspNetCore.Authorization;

namespace Nurse_Recording_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PatientRecordsController : ControllerBase
{
    private readonly AppDbContext _context;

    public PatientRecordsController(AppDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PatientRecord>>> GetPatientRecords()
    {
        return await _context.PatientRecords.Include(pr => pr.Patient).ToListAsync();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PatientRecord>> GetPatientRecord(int id)
    {
        var record = await _context.PatientRecords.Include(pr => pr.Patient).FirstOrDefaultAsync(pr => pr.Id == id);
        if (record == null) return NotFound();
        return record;
    }

    [HttpPost]
    public async Task<ActionResult<PatientRecord>> PostPatientRecord(PatientRecord record)
    {
        _context.PatientRecords.Add(record);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetPatientRecord), new { id = record.Id }, record);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutPatientRecord(int id, PatientRecord record)
    {
        if (id != record.Id) return BadRequest();
        _context.Entry(record).State = EntityState.Modified;
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!PatientRecordExists(id)) return NotFound();
            throw;
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatientRecord(int id)
    {
        var record = await _context.PatientRecords.FindAsync(id);
        if (record == null) return NotFound();
        _context.PatientRecords.Remove(record);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    private bool PatientRecordExists(int id) => _context.PatientRecords.Any(e => e.Id == id);
}
