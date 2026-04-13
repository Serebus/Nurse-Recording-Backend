using System.ComponentModel.DataAnnotations;

namespace Nurse_Recording_Backend.Models;

public enum AlarmState
{
    Idle,
    Calling,
    Coming,
    Ended
}
