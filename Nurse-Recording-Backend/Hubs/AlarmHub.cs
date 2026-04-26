using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Nurse_Recording_Backend.Data;
using Nurse_Recording_Backend.Models;

namespace Nurse_Recording_Backend.Hubs;

public class AlarmHub : Hub
{
    private readonly AppDbContext _context;

    public AlarmHub(AppDbContext context)
    {
        _context = context;
    }

    public override async Task OnConnectedAsync()
    {
        // Send all current alarm states to the newly connected client
        var latestAlarms = await _context.Alarms
            .GroupBy(a => a.DeviceId)
            .Select(g => g.OrderByDescending(a => a.Timestamp).FirstOrDefault())
            .ToListAsync();

        var snapshot = latestAlarms.Select(a => new
        {
            DeviceId = a!.DeviceId,
            State = (int)a.State,
        });

        await Clients.Caller.SendAsync("AlarmSnapshot", snapshot);
        await base.OnConnectedAsync();
    }

    public async Task SendStateChange(AlarmState state, string deviceId = null)
    {
        var alarm = new 
        {
            State = state.ToString(),
            Timestamp = DateTime.UtcNow,
            DeviceId = deviceId
        };
        await Clients.All.SendAsync("ReceiveStateChange", alarm);
    }
}
