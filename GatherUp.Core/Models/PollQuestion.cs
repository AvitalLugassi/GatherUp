using GatherUp.Core.Interfaces;

namespace GatherUp.Core.Models;

public class PollQuestion : IIdentifiable
{
    public Guid Id { get; set; } = Guid.NewGuid();//was init
    public string QuestionText { get; set; } = string.Empty;
    public List<string> Options { get; set; } = [];

    [System.Xml.Serialization.XmlIgnore]
    public Dictionary<Guid, string> VotesByParticipant { get; set; } = [];
}
