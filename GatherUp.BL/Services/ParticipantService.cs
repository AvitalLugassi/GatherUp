using GatherUp.Core.Exceptions;
using GatherUp.Core.Interfaces;
using GatherUp.Core.Models;

namespace GatherUp.BL.Services;

public class ParticipantService(IRepository<GatherEvent> eventRepo)
{
    /// <summary>
    /// הוספת משתתף לאירוע.
    /// </summary>
    public Participant AddParticipant(Guid eventId, Participant participant)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new NotFoundException($"אירוע {eventId} לא נמצא.");

        if (string.IsNullOrWhiteSpace(participant.Name))
            throw new ValidationException("שם המשתתף הוא שדה חובה.");
        if (string.IsNullOrWhiteSpace(participant.Email))
            throw new ValidationException("אימייל המשתתף הוא שדה חובה.");

        if (ev.Participants.Any(p => p.Email == participant.Email))
            throw new BusinessRuleException($"משתתף עם האימייל '{participant.Email}' כבר קיים באירוע.");

        ev.Participants.Add(participant);
        eventRepo.Update(ev);
        return participant;
    }

    public IEnumerable<Participant> GetParticipants(Guid eventId)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new NotFoundException($"אירוע {eventId} לא נמצא.");
        return ev.Participants;
    }

    /// <summary>
    /// עדכון RSVP — אישור או דחיית הגעה.
    /// </summary>
    public void UpdateRsvp(Guid eventId, Guid participantId, bool isAttending)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new NotFoundException($"אירוע {eventId} לא נמצא.");

        var participant = ev.Participants.FirstOrDefault(p => p.Id == participantId)
            ?? throw new NotFoundException($"משתתף {participantId} לא נמצא.");

        participant.IsAttending = isAttending;
        eventRepo.Update(ev);
    }
}
