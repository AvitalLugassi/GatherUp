using GatherUp.API.Endpoints;
using GatherUp.Core.Interfaces;
using GatherUp.Core.Models;
using GatherUp.Core.Services;
using GatherUp.Infrastructure.Email;
using GatherUp.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --- Data paths ---
var dataPath = Path.Combine(AppContext.BaseDirectory, "Data");
Directory.CreateDirectory(dataPath);

// --- Repositories ---
builder.Services.AddSingleton<IRepository<GatherEvent>>(
    _ => new XmlRepository<GatherEvent>(Path.Combine(dataPath, "events.xml")));

// --- Infrastructure ---
builder.Services.AddSingleton<IEmailService, EmailService>();

// --- Services ---
builder.Services.AddScoped<EventService>();
builder.Services.AddScoped<PollService>();
builder.Services.AddScoped<FinanceService>();
builder.Services.AddScoped<NotificationService>();

var app = builder.Build();

// --- Static files (HTML pages) ---
app.UseStaticFiles();

// --- Endpoints ---
app.MapEventEndpoints();
app.MapParticipantEndpoints();
app.MapPollEndpoints();
app.MapVendorEndpoints();

app.Run();
