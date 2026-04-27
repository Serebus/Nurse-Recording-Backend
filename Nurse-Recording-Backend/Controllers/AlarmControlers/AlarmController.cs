using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Nurse_Recording_Backend.Data;
using Nurse_Recording_Backend.Hubs;
using Nurse_Recording_Backend.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Nurse_Recording_Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Nurse,Admin,IotAdmin")]
public class AlarmController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly IHubContext<AlarmHub> _hubContext;

    public AlarmController(AppDbContext context, IHubContext<AlarmHub> hubContext)
    {
        _context = context;
        _hubContext = hubContext;
    }

    [HttpPatch("{deviceId}")]
    public async Task<IActionResult> UpdateState(string deviceId, [FromBody] UpdateAlarmStateRequest request)
    {
        // Validate that the device is registered
        var deviceExists = await _context.Devices.AnyAsync(d => d.DeviceId == deviceId);
        if (!deviceExists)
        {
            return NotFound(new { message = "Device not registered", DeviceId = deviceId });
        }

        var alarm = new Alarm
        {
            State = request.State,
            DeviceId = deviceId,
            Timestamp = DateTime.UtcNow
            // NurseId from claims if needed
        };

        _context.Alarms.Add(alarm);
        await _context.SaveChangesAsync();

        // In UpdateState(), replace the SendAsync call with:
await _hubContext.Clients.All.SendAsync("AlarmUpdated", new
{
    DeviceId = alarm.DeviceId,
    State    = (int)alarm.State,    // ← int, not .ToString()
});

        return Ok(alarm);
    }

    [HttpGet]
    public async Task<IActionResult> GetStatus()
    {
        var latestAlarms = await _context.Alarms
            .GroupBy(a => a.DeviceId)
            .Select(g => g.OrderByDescending(a => a.Timestamp).FirstOrDefault())
            .ToListAsync();

        return Ok(latestAlarms);
    }

    [HttpPost("newDevice")]
    public async Task<IActionResult> GetStatusPost([FromBody] GetStatusRequest request)
    {
        var device = await _context.Devices.FirstOrDefaultAsync(d => d.DeviceId == request.DeviceId);

        if (device == null)
        {
            // 1. Register the device first
            device = new Device { DeviceId = request.DeviceId, Description = "Auto-registered" };
            _context.Devices.Add(device);
            await _context.SaveChangesAsync();

            // 2. Add initial Idle record after device exists
            var initialAlarm = new Alarm { DeviceId = request.DeviceId, State = AlarmState.Idle, Timestamp = DateTime.UtcNow };
            _context.Alarms.Add(initialAlarm);
            await _context.SaveChangesAsync();

            return Ok(initialAlarm);
        }

        var alarm = await _context.Alarms
            .Where(a => a.DeviceId == request.DeviceId)
            .OrderByDescending(a => a.Timestamp)
            .FirstOrDefaultAsync();

        if (alarm == null)
        {
            // This case should be rare now since we add an initial record upon registration
            alarm = new Alarm { DeviceId = request.DeviceId, State = AlarmState.Idle, Timestamp = DateTime.UtcNow };
            _context.Alarms.Add(alarm);
            await _context.SaveChangesAsync();
        }

        return Ok(alarm);
    }
}

public class GetStatusRequest
{
    [Required]
    public string DeviceId { get; set; } = string.Empty;
}

public class UpdateAlarmStateRequest
{
    [Required]
    public AlarmState State { get; set; }
}
