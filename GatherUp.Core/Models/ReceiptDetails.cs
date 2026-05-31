namespace GatherUp.Core.Models;

public record ReceiptDetails(
    string ReceiptNumber,
    decimal Amount,
    DateTime IssuedAt
);
