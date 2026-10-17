namespace Clinic.Api.Models;

public record Appointment(int Id, int DoctorId, string PatientName, DateTime Start, DateTime End);

public record BookingRequest(int DoctorId, string PatientName, DateTime Start, int Minutes);
