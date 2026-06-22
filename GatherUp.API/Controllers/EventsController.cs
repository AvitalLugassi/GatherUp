using GatherUp.BL.Services;
using GatherUp.Core.Enums;
using GatherUp.Core.Exceptions;
using GatherUp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace GatherUp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class EventsController(EventService eventService, NotificationService notificationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll() => Ok(eventService.GetAll());

    [AllowAnonymous]
    [HttpGet("{id:guid}")]
    public IActionResult GetById(Guid id)
    {
        var ev = eventService.GetById(id);
        return ev is null
            ? throw new NotFoundException($"אירוע {id} לא נמצא.")
            : Ok(ev);
    }

    [HttpPost]
    public IActionResult Create(GatherEvent gatherEvent)
    {
        var created = eventService.Create(gatherEvent);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public IActionResult Update(Guid id, GatherEvent gatherEvent)
    {
        if (id != gatherEvent.Id)
            throw new ValidationException("מזהה האירוע אינו תואם.");
        if (!IsOwnerOrAdmin(id)) return Forbid();
        return Ok(eventService.Update(gatherEvent));
    }

    [Authorize(Roles = "Admin")]
    [HttpDelete("{id:guid}")]
    public IActionResult Delete(Guid id)
    {
        eventService.Delete(id);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public IActionResult UpdateStatus(Guid id, [FromBody] EventStatus newStatus)
    {
        if (!IsOwnerOrAdmin(id)) return Forbid();
        eventService.UpdateStatus(id, newStatus);
        return NoContent();
    }

    [HttpPost("{id:guid}/send-invitations")]
    public IActionResult SendInvitations(Guid id)
    {
        notificationService.SendInvitations(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/send-update")]
    public IActionResult SendUpdate(Guid id, [FromBody] string message)
    {
        notificationService.SendEventUpdate(id, message);
        return NoContent();
    }

    // --- helpers ---

    private Guid CurrentUserId() =>
        Guid.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

    private bool IsOwnerOrAdmin(Guid eventId)
    {
        var ev = eventService.GetById(eventId);
        return ev?.Host?.Id == CurrentUserId() || User.IsInRole("Admin");
    }
}
