using Clinic.Api.Data;

namespace Clinic.Api.Endpoints;

public static class ClinicEndpoints
{
    public static void MapClinicEndpoints(this WebApplication app)
    {
        app.MapGet("/api/doctors", (InMemoryStore store) => store.GetDoctors());

        app.MapGet("/api/appointments", (InMemoryStore store) => store.GetAppointments());

        app.MapPost("/api/appointments", (BookingRequest request, InMemoryStore store) =>
        {
            if (string.IsNullOrWhiteSpace(request.PatientName) || request.Minutes <= 0)
            {
                return Results.BadRequest(new { message = "Patient name and a valid length are required." });
            }

            try
            {
                var appointment = store.Book(request.DoctorId, request.PatientName.Trim(), request.Start, request.Minutes);
                return appointment is null
                    ? Results.NotFound(new { message = "Doctor not found." })
                    : Results.Ok(appointment);
            }
            catch (ArgumentException ex)
            {
                return Results.BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Results.Conflict(new { message = ex.Message });
            }
        });
    }
}
