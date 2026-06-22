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
        => Ok(eventService.SetHost(eventId, host));

    [HttpPost("managers")]
    public IActionResult AddManager(Guid eventId, EventManager manager)
        => Ok(eventService.AddManager(eventId, manager));

    [HttpDelete("managers/{managerId:guid}")]
    public IActionResult RemoveManager(Guid eventId, Guid managerId)
    {
        eventService.RemoveManager(eventId, managerId);
        return NoContent();
    }
}
