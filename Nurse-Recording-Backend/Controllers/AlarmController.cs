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

    [HttpPatch]
    public async Task<IActionResult> UpdateState([FromBody] UpdateAlarmStateRequest request)
    {
        var alarm = new Alarm
        {
            State = request.State,
            DeviceId = request.DeviceId,
            Timestamp = DateTime.UtcNow
            // NurseId from claims if needed
        };

        _context.Alarms.Add(alarm);
        await _context.SaveChangesAsync();

        await _hubContext.Clients.All.SendAsync("ReceiveStateChange", new 
        {
            State = alarm.State.ToString(),
            Timestamp = alarm.Timestamp,
            DeviceId = alarm.DeviceId
        });

        return Ok(alarm);
    }

    [HttpGet("{deviceId}")]
    public async Task<IActionResult> GetStatus(string deviceId)
    {
        var alarm = await _context.Alarms
            .Where(a => a.DeviceId == deviceId)
            .OrderByDescending(a => a.Timestamp)
            .FirstOrDefaultAsync();

        if (alarm == null)
            return NotFound(new { message = "No alarm history for this device", DeviceId = deviceId, State = AlarmState.Idle });

        return Ok(alarm);
    }

    [HttpPost("status")]
    public async Task<IActionResult> GetStatusPost([FromBody] GetStatusRequest request)
    {
        var alarm = await _context.Alarms
            .Where(a => a.DeviceId == request.DeviceId)
            .OrderByDescending(a => a.Timestamp)
            .FirstOrDefaultAsync();

        if (alarm == null)
            return Ok(new { DeviceId = request.DeviceId, State = AlarmState.Idle, message = "Defaulting to Idle" });

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
    public string? DeviceId { get; set; }
}
