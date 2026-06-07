using GatherUp.Core.Interfaces;

namespace GatherUp.Core.Models;

public class VendorAllocation : IIdentifiable
{
    public Guid Id { get; set; } = Guid.NewGuid();//was init
    public string Name { get; set; } = string.Empty;
    public decimal AmountOwed { get; set; }
    [System.Xml.Serialization.XmlIgnore]
    public bool HasReceipt => Receipts.Count > 0;

    [System.Xml.Serialization.XmlIgnore]
    public List<ReceiptDetails> Receipts { get; set; } = [];
}
