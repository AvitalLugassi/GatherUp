using GatherUp.BL.Services;
using GatherUp.Core.Interfaces;
using GatherUp.Core.Models;
using GatherUp.Infrastructure.Repositories;

namespace GatherUp.BL;

/// <summary>
/// נקודת כניסה מרכזית לשכבת ה-BL.
/// מאגד את כל ה-Services יחד ומספק גישה נוחה אליהם.
/// </summary>
public class GatherUpCore
{
    public EventService EventService { get; }
    public PollService PollService { get; }
    public FinanceService FinanceService { get; }
    public NotificationService NotificationService { get; }
    public ParticipantService ParticipantService { get; }

    public GatherUpCore(IRepository<GatherEvent> repo, IEmailService emailService, VotesXmlRepository votesRepo)
    {
        EventService = new EventService(repo);
        PollService = new PollService(repo, votesRepo);
        FinanceService = new FinanceService(repo);
        NotificationService = new NotificationService(emailService, repo);
        ParticipantService = new ParticipantService(repo);
    }
}
