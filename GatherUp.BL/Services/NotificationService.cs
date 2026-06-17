using GatherUp.Core.Enums;
using GatherUp.Core.Interfaces;
using GatherUp.Core.Models;

namespace GatherUp.BL.Services;

public class NotificationService(IEmailService emailService, IRepository<GatherEvent> eventRepo)
{
    /// <summary>
    /// שליחת הזמנות לכל המשתתפים שבחרו לקבל עדכונים על שינויי אירוע.
    /// </summary>
    public void SendInvitations(Guid eventId)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

        if (ev.Host is null)
            throw new InvalidOperationException("לא ניתן לשלוח הזמנות ללא בעל אירוע.");

        var recipients = ev.Participants
            .Where(p => p.NotificationPreferences.HasFlag(NotificationPreference.EventChanges))
            .Select(p => p.Email)
            .ToList();

        if (recipients.Count == 0) return;

        var subject = $"הזמנה: {ev.Title}";
        var body = BuildInvitationBody(ev);

        emailService.SendBulk(recipients, subject, body);

        ev.Status = EventStatus.InvitationsSent;
        eventRepo.Update(ev);
    }

    /// <summary>
    /// שליחת תזכורות תשלום למשתתפים שעדיין לא שילמו.
    /// </summary>
    public void SendPaymentReminders(Guid eventId)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

        var unpaid = ev.Participants
            .Where(p => !p.HasPaid && p.IsAttending == true)
            .ToList();

        foreach (var participant in unpaid)
        {
            var subject = $"תזכורת תשלום - {ev.Title}";
            var body = $"שלום {participant.Name},\n\nהסכום לתשלום: {ev.PricePerParticipant}₪\n\nתודה!";
            emailService.Send(participant.Email, subject, body);
        }
    }

    /// <summary>
    /// שליחת עדכון לכל המשתתפים שבחרו לקבל שינויי אירוע.
    /// </summary>
    public void SendEventUpdate(Guid eventId, string updateMessage)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

        var recipients = ev.Participants
            .Where(p => p.NotificationPreferences.HasFlag(NotificationPreference.EventChanges))
            .Select(p => p.Email)
            .ToList();

        if (recipients.Count == 0) return;

        emailService.SendBulk(recipients, $"עדכון: {ev.Title}", updateMessage);
    }

    /// <summary>
    /// שליחת הודעה על סקר חדש למשתתפים שביקשו עדכון על סקרים.
    /// </summary>
    public void NotifyNewPoll(Guid eventId, Poll poll)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

        var recipients = ev.Participants
            .Where(p => p.NotificationPreferences.HasFlag(NotificationPreference.NewPolls))
            .Select(p => p.Email)
            .ToList();

        if (recipients.Count == 0) return;

        var subject = $"סקר חדש: {poll.Title}";
        var body = $"נפתח סקר חדש '{poll.Title}' לאירוע '{ev.Title}'.\nהסקר פתוח עד: {poll.ClosesAt?.ToShortDateString()}";
        emailService.SendBulk(recipients, subject, body);
    }

    private static string BuildInvitationBody(GatherEvent ev)
    {
        var lines = new List<string>
        {
            $"הזמנה לאירוע: {ev.Title}",
            $"תאריך: {ev.EventDate?.ToShortDateString()}",
            $"מיקום: {ev.Location}"
        };

        if (!string.IsNullOrWhiteSpace(ev.CustomMessage))
            lines.Add(ev.CustomMessage);

        if (ev.PricePerParticipant.HasValue)
            lines.Add($"עלות השתתפות: {ev.PricePerParticipant}₪");

        return string.Join("\n", lines);
    }
}
