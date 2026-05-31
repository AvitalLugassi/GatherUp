namespace GatherUp.API.Endpoints;

public static class PollEndpoints
{
    public static void MapPollEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/events/{eventId}/polls");

        // TODO: POST            - יצירת סקר
        // TODO: GET             - רשימת סקרים
        // TODO: POST /{pollId}/vote - הצבעה
        // TODO: GET  /{pollId}/results - תוצאות
    }
}
