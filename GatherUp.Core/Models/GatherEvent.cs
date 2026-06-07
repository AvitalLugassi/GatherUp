using GatherUp.Core.Enums;
using GatherUp.Core.Interfaces;

namespace GatherUp.Core.Models;

public class GatherEvent : IIdentifiable
{
    public Guid Id { get; set; } = Guid.NewGuid(); //was init
    public string Title { get; set; } = string.Empty;
    public DateTime? EventDate { get; set; }
    public string? Location { get; set; }
    public decimal? PricePerParticipant { get; set; }
    public string? CustomMessage { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Draft;
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.BankTransfer;
    public string? BankDetails { get; set; }
    public string? CashContactName { get; set; }

    public EventHost? Host { get; set; }
    public List<EventManager> Managers { get; set; } = [];
    public List<Participant> Participants { get; set; } = [];
    public List<VendorAllocation> Vendors { get; set; } = [];
    public List<Poll> Polls { get; set; } = [];

    [System.Xml.Serialization.XmlIgnore]
    public decimal TotalCollected => Participants
        .Where(p => p.HasPaid)
        .Sum(p => p.AmountPaid);

    [System.Xml.Serialization.XmlIgnore]
    public decimal TotalOwedToVendors => Vendors.Sum(v => v.AmountOwed);

    [System.Xml.Serialization.XmlIgnore]
    public decimal Budget => TotalCollected - TotalOwedToVendors;
}
