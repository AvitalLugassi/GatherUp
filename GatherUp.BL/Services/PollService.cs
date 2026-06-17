using GatherUp.Core.Interfaces;
using GatherUp.Core.Models;

namespace GatherUp.BL.Services;

public class PollService(IRepository<GatherEvent> eventRepo)
{
    public Poll AddPoll(Guid eventId, Poll poll)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

        if (string.IsNullOrWhiteSpace(poll.Title))
            throw new ArgumentException("כותרת הסקר היא שדה חובה.");

        ev.Polls.Add(poll);
        eventRepo.Update(ev);
        return poll;
    }

    public IEnumerable<Poll> GetPolls(Guid eventId)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");
        return ev.Polls;
    }

    /// <summary>
    /// רישום הצבעה של משתתף על שאלה בסקר.
    /// </summary>
    public void Vote(Guid eventId, Guid pollId, Guid questionId, Guid participantId, string answer)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

        var poll = ev.Polls.FirstOrDefault(p => p.Id == pollId)
            ?? throw new KeyNotFoundException($"סקר {pollId} לא נמצא.");

        if (poll.IsClosed)
            throw new InvalidOperationException("הסקר סגור ולא ניתן להצביע.");

        var question = poll.Questions.FirstOrDefault(q => q.Id == questionId)
            ?? throw new KeyNotFoundException($"שאלה {questionId} לא נמצאה.");

        if (!question.Options.Contains(answer))
            throw new ArgumentException($"תשובה '{answer}' אינה אפשרות חוקית.");

        question.VotesByParticipant[participantId] = answer;

        // VotesByParticipant הוא XmlIgnore — נשמר רק ב-memory בזמן ריצה.
        // לשמירה מתמשכת יש להשתמש ב-VoteXmlRepository (ניתן להרחיב בעתיד).
        eventRepo.Update(ev);
    }

    public Dictionary<string, int> GetResults(Guid eventId, Guid pollId, Guid questionId)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

        var poll = ev.Polls.FirstOrDefault(p => p.Id == pollId)
            ?? throw new KeyNotFoundException($"סקר {pollId} לא נמצא.");

        var question = poll.Questions.FirstOrDefault(q => q.Id == questionId)
            ?? throw new KeyNotFoundException($"שאלה {questionId} לא נמצאה.");

        return question.VotesByParticipant
            .GroupBy(v => v.Value)
            .ToDictionary(g => g.Key, g => g.Count());
    }
}
