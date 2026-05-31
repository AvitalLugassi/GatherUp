namespace GatherUp.Core.Models;

public class Poll : IIdentifiable
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime? ClosesAt { get; set; }
    public bool IsClosed => ClosesAt.HasValue && DateTime.UtcNow > ClosesAt.Value;
    public List<PollQuestion> Questions { get; set; } = [];
}
