using GatherUp.Core.Interfaces;
using GatherUp.Core.Models;
using GatherUp.Core.Services;

namespace GatherUp.Core;

public class GatherUpCore
{
    public EventService EventService { get; }
    public PollService PollService { get; }
    public FinanceService FinanceService { get; }
    public NotificationService NotificationService { get; }

    public GatherUpCore(IRepository<GatherEvent> repo, IEmailService emailService)
    {
        EventService = new EventService(repo);
        PollService = new PollService(repo);
        FinanceService = new FinanceService(repo);
        NotificationService = new NotificationService(emailService, repo);
    }
}
