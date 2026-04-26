using Microsoft.AspNetCore.SignalR;
using Nurse_Recording_Backend.Data;
using Nurse_Recording_Backend.Models;
using Microsoft.EntityFrameworkCore;

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
            State    = (int)a.State,      // ← send as int, not string
        });

        await Clients.Caller.SendAsync("AlarmSnapshot", snapshot);
        await base.OnConnectedAsync();
    }
}