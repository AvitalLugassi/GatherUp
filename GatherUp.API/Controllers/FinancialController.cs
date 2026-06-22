using GatherUp.BL.Services;
using GatherUp.Core.Enums;
using GatherUp.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GatherUp.API.Controllers;

[ApiController]
[Route("api/events/{eventId:guid}/financial")]
[Authorize]
public class FinancialController(FinanceService financeService) : ControllerBase
{
    [HttpGet("vendors")]
    public IActionResult GetVendors(Guid eventId) => Ok(financeService.GetVendors(eventId));

    [HttpPost("vendors")]
    public IActionResult AddVendor(Guid eventId, VendorAllocation vendor)
    {
        var added = financeService.AddVendor(eventId, vendor);
        return CreatedAtAction(nameof(GetVendors), new { eventId }, added);
    }

    [HttpDelete("vendors/{vendorId:guid}")]
    public IActionResult DeleteVendor(Guid eventId, Guid vendorId)
    {
        financeService.DeleteVendor(eventId, vendorId);
        return NoContent();
    }

    [HttpPost("vendors/{vendorId:guid}/receipt")]
    public IActionResult AttachReceipt(Guid eventId, Guid vendorId, ReceiptDetails receipt)
    {
        financeService.AttachReceipt(eventId, vendorId, receipt);
        return NoContent();
    }

    [HttpPost("payments/{participantId:guid}")]
    public IActionResult MarkPayment(Guid eventId, Guid participantId, [FromBody] PaymentRequest request)
    {
        financeService.MarkPayment(eventId, participantId, request.Amount, request.Method);
        return NoContent();
    }
}

public record PaymentRequest(decimal Amount, PaymentMethod Method);
