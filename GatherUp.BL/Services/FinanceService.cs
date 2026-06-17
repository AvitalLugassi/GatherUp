using GatherUp.Core.Enums;
using GatherUp.Core.Interfaces;
using GatherUp.Core.Models;

namespace GatherUp.BL.Services;

public class FinanceService(IRepository<GatherEvent> eventRepo)
{
    /// <summary>
    /// סימון תשלום עבור משתתף.
    /// </summary>
    public void MarkPayment(Guid eventId, Guid participantId, decimal amount, PaymentMethod method)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

        var participant = ev.Participants.FirstOrDefault(p => p.Id == participantId)
            ?? throw new KeyNotFoundException($"משתתף {participantId} לא נמצא.");

        if (amount <= 0)
            throw new ArgumentException("סכום התשלום חייב להיות חיובי.");

        // בדיקה שאמצעי התשלום תואם את שנקבע באירוע
        if (ev.PaymentMethod != method)
            throw new InvalidOperationException(
                $"אמצעי התשלום '{method}' אינו תואם את שיטת התשלום שנקבעה לאירוע '{ev.PaymentMethod}'.");

        participant.HasPaid = true;
        participant.AmountPaid = amount;
        eventRepo.Update(ev);
    }

    /// <summary>
    /// הוספת ספק לאירוע.
    /// </summary>
    public VendorAllocation AddVendor(Guid eventId, VendorAllocation vendor)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

        if (string.IsNullOrWhiteSpace(vendor.Name))
            throw new ArgumentException("שם הספק הוא שדה חובה.");

        if (vendor.AmountOwed < 0)
            throw new ArgumentException("הסכום המגיע לספק לא יכול להיות שלילי.");

        ev.Vendors.Add(vendor);
        eventRepo.Update(ev);
        return vendor;
    }

    /// <summary>
    /// צירוף קבלה לספק — הקבלה נשמרת ב-VendorAllocation.Receipts בזמן ריצה.
    /// לשמירה מתמשכת יש להשתמש ב-ReceiptXmlRepository.
    /// </summary>
    public void AttachReceipt(Guid eventId, Guid vendorId, ReceiptDetails receipt)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

        var vendor = ev.Vendors.FirstOrDefault(v => v.Id == vendorId)
            ?? throw new KeyNotFoundException($"ספק {vendorId} לא נמצא.");

        vendor.Receipts.Add(receipt);
        eventRepo.Update(ev);
    }

    public IEnumerable<VendorAllocation> GetVendors(Guid eventId)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");
        return ev.Vendors;
    }

    public void DeleteVendor(Guid eventId, Guid vendorId)
    {
        var ev = eventRepo.GetById(eventId)
            ?? throw new KeyNotFoundException($"אירוע {eventId} לא נמצא.");

        var vendor = ev.Vendors.FirstOrDefault(v => v.Id == vendorId)
            ?? throw new KeyNotFoundException($"ספק {vendorId} לא נמצא באירוע.");

        ev.Vendors.Remove(vendor);
        eventRepo.Update(ev);
    }
}
