using Microsoft.AspNetCore.SignalR;
using Nurse_Recording_Backend.Models;

namespace Nurse_Recording_Backend.Hubs;

public class AlarmHub : Hub
{
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
