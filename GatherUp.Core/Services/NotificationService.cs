using GatherUp.Core.Interfaces;
using GatherUp.Core.Models;

namespace GatherUp.Core.Services;

public class NotificationService(IEmailService emailService, IRepository<GatherEvent> eventRepo)
{
    // TODO: שליחת הזמנות, תזכורות תשלום, עדכוני אירוע לפי העדפות
}
