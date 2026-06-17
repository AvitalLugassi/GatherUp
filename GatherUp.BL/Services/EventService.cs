using GatherUp.Core.Enums;
using GatherUp.Core.Interfaces;
using GatherUp.Core.Models;

namespace GatherUp.BL.Services;

public class EventService(IRepository<GatherEvent> eventRepo)
{
    public IEnumerable<GatherEvent> GetAll() => eventRepo.GetAll();

    public GatherEvent? GetById(Guid id) => eventRepo.GetById(id);

    public GatherEvent Create(GatherEvent gatherEvent)
    {
        if (string.IsNullOrWhiteSpace(gatherEvent.Title))
            throw new ArgumentException("כותרת האירוע היא שדה חובה.");
        if (gatherEvent.EventDate is null)
            throw new ArgumentException("תאריך האירוע הוא שדה חובה.");
        if (gatherEvent.Location is null || string.IsNullOrWhiteSpace(gatherEvent.Location))
            throw new ArgumentException("מיקום האירוע הוא שדה חובה.");

        eventRepo.Add(gatherEvent);
        return gatherEvent;
    }

    public GatherEvent Update(GatherEvent gatherEvent)
    {
        if (string.IsNullOrWhiteSpace(gatherEvent.Title))
            throw new ArgumentException("כותרת האירוע היא שדה חובה.");

        eventRepo.Update(gatherEvent);
        return gatherEvent;
    }

    public void Delete(Guid id) => eventRepo.Delete(id);

    public void UpdateStatus(Guid id, EventStatus newStatus)
    {
        var ev = eventRepo.GetById(id)
            ?? throw new KeyNotFoundException($"אירוע {id} לא נמצא.");
        ev.Status = newStatus;
        eventRepo.Update(ev);
    }
}
