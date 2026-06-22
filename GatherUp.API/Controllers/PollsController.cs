using GatherUp.BL.Services;
using GatherUp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers;

[ApiController]
[Route("api/events/{eventId:guid}/polls")]
[Authorize]
public class PollsController(PollService pollService, NotificationService notificationService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetAll(Guid eventId) => Ok(pollService.GetPolls(eventId));

    [HttpPost]
    public IActionResult Create(Guid eventId, Poll poll)
    {
        var created = pollService.AddPoll(eventId, poll);
        notificationService.NotifyNewPoll(eventId, created);
        return CreatedAtAction(nameof(GetAll), new { eventId }, created);
    }

    [HttpPost("{pollId:guid}/questions/{questionId:guid}/vote")]
    public IActionResult Vote(Guid eventId, Guid pollId, Guid questionId, [FromBody] VoteRequest request)
    {
        pollService.Vote(eventId, pollId, questionId, request.ParticipantId, request.Answer);
        return NoContent();
    }

    [HttpGet("{pollId:guid}/questions/{questionId:guid}/results")]
    public IActionResult GetResults(Guid eventId, Guid pollId, Guid questionId)
        => Ok(pollService.GetResults(eventId, pollId, questionId));
}

public record VoteRequest(Guid ParticipantId, string Answer);
