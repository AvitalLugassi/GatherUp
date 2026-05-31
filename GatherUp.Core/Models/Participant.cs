using GatherUp.Core.Enums;

namespace GatherUp.Core.Models;

public class Participant : Person
{
    public bool? IsAttending { get; set; } = null;  // null = טרם השיב
    public bool HasPaid { get; set; } = false;
    public decimal AmountPaid { get; set; } = 0;
    public NotificationPreference NotificationPreferences { get; set; } = NotificationPreference.None;
}
