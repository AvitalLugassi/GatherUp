namespace GatherUp.API.Endpoints;

public static class EventEndpoints
{
    public static void MapEventEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/events");

        // TODO: GET /api/events
        // TODO: POST /api/events
        // TODO: GET /api/events/{id}
        // TODO: PUT /api/events/{id}
        // TODO: DELETE /api/events/{id}
        // TODO: POST /api/events/{id}/send-invitations
    }
}
