using GatherUp.Core.Enums;
using GatherUp.Core.Exceptions;
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
            ?? throw new NotFoundException($"אירוע {eventId} לא נמצא.");

        if (ev.Host is null)
            throw new BusinessRuleException("לא ניתן לשלוח הזמנות ללא בעל אירוע.");

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
            ?? throw new NotFoundException($"אירוע {eventId} לא נמצא.");

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
    /// שליחת עדכון לכל המשתתפים (ללא קשר להעדפות התראה).
    /// </summary>
    public void SendEventUpdate(Guid eventId, string updateMessage)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new NotFoundException($"אירוע {eventId} לא נמצא.");

        var recipients = ev.Participants
            .Where(p => !string.IsNullOrWhiteSpace(p.Email))
            .Select(p => p.Email)
            .ToList();

        if (recipients.Count == 0) return;

        emailService.SendBulk(recipients, $"עדכון: {ev.Title}", updateMessage);
    }

    public void NotifyNewPoll(Guid eventId, Poll poll)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new NotFoundException($"אירוע {eventId} לא נמצא.");

        var recipients = ev.Participants
            .Where(p => p.NotificationPreferences.HasFlag(NotificationPreference.NewPolls))
            .Select(p => p.Email)
            .ToList();

        if (recipients.Count == 0) return;

        var subject = $"סקר חדש: {poll.Title}";
        var body = $"נפתח סקר חדש '{poll.Title}' לאירוע '{ev.Title}'.\nהסקר פתוח עד: {poll.ClosesAt?.ToShortDateString()}";
        emailService.SendBulk(recipients, subject, body);
    }

    /// <summary>
    /// שולח למשתתף מייל ברגע שנרשם לאירוע, עם שם המשתמש והסיסמה שלו.
    /// </summary>
    public void SendWelcomeToParticipant(Guid eventId, Participant participant)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new NotFoundException($"אירוע {eventId} לא נמצא.");

        if (string.IsNullOrWhiteSpace(participant.Email)) return;

        var subject = $"הוזמנת לאירוע: {ev.Title}";
        var body = $@"<div dir='rtl'>
<h2>שלום {participant.Name},</h2>
<p>נרשמת לאירוע <strong>{ev.Title}</strong>.</p>
{(ev.EventDate.HasValue ? $"<p>📅 תאריך: {ev.EventDate.Value.ToShortDateString()}</p>" : "")}
{(!string.IsNullOrWhiteSpace(ev.Location) ? $"<p>📍 מיקום: {ev.Location}</p>" : "")}
{(ev.PricePerParticipant.HasValue ? $"<p>💰 עלות: ₪{ev.PricePerParticipant}</p>" : "")}
{(!string.IsNullOrWhiteSpace(ev.CustomMessage) ? $"<p>{ev.CustomMessage}</p>" : "")}
<hr/>
<p><strong>פרטי כניסה למערכת:</strong></p>
<p>שם משתמש: {participant.Email}</p>
<p>(הסיסמה שלך נשלחה אליך בנפרד, אם נוצר עבורך חשבון)</p>
</div>";

        emailService.Send(participant.Email, subject, body);
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
