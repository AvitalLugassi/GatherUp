namespace GatherUp.API.Endpoints;

public static class ParticipantEndpoints
{
    public static void MapParticipantEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/events/{eventId}/participants");

        // TODO: POST   - הוספת משתתף
        // TODO: GET    - רשימת משתתפים
        // TODO: PUT /{participantId}/rsvp     - אישור/דחיית הגעה
        // TODO: PUT /{participantId}/payment  - סימון תשלום
    }
}
