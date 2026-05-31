namespace GatherUp.API.Endpoints;

public static class VendorEndpoints
{
    public static void MapVendorEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/events/{eventId}/vendors");

        // TODO: POST            - הוספת ספק
        // TODO: GET             - רשימת ספקים
        // TODO: PUT /{vendorId}/receipt - צירוף קבלה
        // TODO: DELETE /{vendorId}      - מחיקת ספק
    }
}
