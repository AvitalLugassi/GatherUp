using GatherUp.BL.Services;
using GatherUp.Core.Models;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers;

[ApiController]
[Route("api/events/{eventId:guid}/persons")]
public class PersonController(EventService eventService) : ControllerBase
{
    [HttpPut("host")]
    public IActionResult SetHost(Guid eventId, EventHost host)
    {
        var ev = eventService.GetById(eventId);
        if (ev is null) return NotFound();
        ev.Host = host;
        eventService.Update(ev);
        return Ok(ev.Host);
    }

    [HttpPost("managers")]
    public IActionResult AddManager(Guid eventId, EventManager manager)
    {
        var ev = eventService.GetById(eventId);
        if (ev is null) return NotFound();
        ev.Managers.Add(manager);
        eventService.Update(ev);
        return Ok(manager);
    }

    [HttpDelete("managers/{managerId:guid}")]
    public IActionResult RemoveManager(Guid eventId, Guid managerId)
    {
        var ev = eventService.GetById(eventId);
        if (ev is null) return NotFound();
        var manager = ev.Managers.FirstOrDefault(m => m.Id == managerId);
        if (manager is null) return NotFound();
        ev.Managers.Remove(manager);
        eventService.Update(ev);
        return NoContent();
    }
}
