using GatherUp.Core;
using GatherUp.Core.Enums;
using GatherUp.Core.Models;
using GatherUp.Infrastructure.Data;
using GatherUp.Infrastructure.Email;
using GatherUp.Infrastructure.Repositories;

// 1. יצירת ה-Core עם MemoryRepository
var repo = new MemoryRepository<GatherEvent>();
var emailService = new EmailService();
var core = new GatherUpCore(repo, emailService);

// 2. אתחול נתוני דמה
InitializeData.Seed(repo);

var eventId = repo.GetAll().First().Id;

// 3. הוספת 3 משתתפים חדשים
var p1 = new Participant { Name = "יוסי כהן",   Email = "yosi@test.com",   NotificationPreferences = NotificationPreference.EventChanges };
var p2 = new Participant { Name = "דינה לוי",   Email = "dina@test.com",   NotificationPreferences = NotificationPreference.NewPolls };
var p3 = new Participant { Name = "אבי מזרחי",  Email = "avi@test.com",    NotificationPreferences = NotificationPreference.DirectMessages };

Initialize.AddParticipantToEvent(repo, eventId, p1);
Initialize.AddParticipantToEvent(repo, eventId, p2);
Initialize.AddParticipantToEvent(repo, eventId, p3);

// שליפת משתתף לפי Id
var found = repo.GetById(eventId)!.Participants.FirstOrDefault(p => p.Id == p1.Id);
Console.WriteLine($"נמצא משתתף: {found?.Name}");

// הדפסת כל המשתתפים
Console.WriteLine("\nרשימת כל המשתתפים:");
foreach (var p in repo.GetById(eventId)!.Participants)
    Console.WriteLine($"- {p.Name} | {p.Email}");
