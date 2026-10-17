using Clinic.Api.Models;

namespace Clinic.Api.Data;

public class InMemoryStore
{
    public const int OpenHour = 9;
    public const int CloseHour = 17;

    private readonly List<Doctor> _doctors = new()
    {
        new Doctor(1, "Dr. Meera Nair", "General Physician"),
        new Doctor(2, "Dr. Rohan Shah", "Dermatologist"),
        new Doctor(3, "Dr. Lena Fischer", "Pediatrician"),
    };

    private readonly List<Appointment> _appointments = new();

    public IReadOnlyList<Doctor> GetDoctors() => _doctors;

    public IReadOnlyList<Appointment> GetAppointments() => _appointments;

    public Appointment? Book(int doctorId, string patientName, DateTime start, int minutes)
    {
        if (!_doctors.Any(d => d.Id == doctorId))
        {
            return null;
        }

        var end = start.AddMinutes(minutes);

        if (start.Hour < OpenHour || end.Hour > CloseHour)
        {
            throw new ArgumentException("The clinic is open from 09:00 to 17:00.");
        }

        var clash = _appointments
            .Where(a => a.DoctorId == doctorId)
            .Any(a => Overlaps(a, start, end));
        if (clash)
        {
            throw new InvalidOperationException("The doctor is already booked at that time.");
        }

        var appointment = new Appointment(_appointments.Count + 1, doctorId, patientName, start, end);
        _appointments.Add(appointment);
        return appointment;
    }

    private static bool Overlaps(Appointment existing, DateTime start, DateTime end)
    {
        return existing.Start < start && start < existing.End;
    }
}
