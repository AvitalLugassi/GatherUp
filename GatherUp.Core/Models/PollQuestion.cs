using GatherUp.Core.Interfaces;

namespace GatherUp.Core.Models;

public class PollQuestion : IIdentifiable
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string QuestionText { get; set; } = string.Empty;
    public List<string> Options { get; set; } = [];
    public Dictionary<Guid, string> VotesByParticipant { get; set; } = [];
}
