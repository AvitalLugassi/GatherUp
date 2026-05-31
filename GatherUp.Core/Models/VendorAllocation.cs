using GatherUp.Core.Interfaces;

namespace GatherUp.Core.Models;

public class VendorAllocation : IIdentifiable
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
    public decimal AmountOwed { get; set; }
    public bool HasReceipt => Receipts.Count > 0;
    public List<ReceiptDetails> Receipts { get; set; } = [];
}
