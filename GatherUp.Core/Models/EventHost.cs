namespace GatherUp.Core.Models;

public class EventHost : Person
{
    public DateTime? InvitationScheduledAt { get; set; }
    public string InvitationContent { get; set; } = string.Empty;
}
