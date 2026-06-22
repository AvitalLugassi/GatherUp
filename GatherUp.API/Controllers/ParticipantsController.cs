using GatherUp.BL.Services;
using GatherUp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers;

[ApiController]
[Route("api/events/{eventId:guid}/participants")]
[Authorize]
public class ParticipantsController(ParticipantService participantService, NotificationService notificationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll(Guid eventId) => Ok(participantService.GetParticipants(eventId));

    [HttpPost]
    public IActionResult Add(Guid eventId, Participant participant)
    {
        var added = participantService.AddParticipant(eventId, participant);
        return CreatedAtAction(nameof(GetAll), new { eventId }, added);
    }

    [HttpPatch("{participantId:guid}/rsvp")]
    public IActionResult UpdateRsvp(Guid eventId, Guid participantId, [FromBody] bool isAttending)
    {
        participantService.UpdateRsvp(eventId, participantId, isAttending);
        return NoContent();
    }

    [HttpPost("send-payment-reminders")]
    public IActionResult SendPaymentReminders(Guid eventId)
    {
        notificationService.SendPaymentReminders(eventId);
        return NoContent();
    }
}
