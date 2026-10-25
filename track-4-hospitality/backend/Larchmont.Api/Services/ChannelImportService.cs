using System.Globalization;
using Larchmont.Api.Domain;
using Larchmont.Api.Dtos;
using Larchmont.Api.Infrastructure;
using Larchmont.Api.Repositories;

namespace Larchmont.Api.Services;

public interface IChannelImportService
{
    Task<ImportResultDto> ImportAsync(string csv);
}

public class ChannelImportService : IChannelImportService
{
    private readonly IBookingRepository _bookings;
    private readonly IGuestRepository _guests;
    private readonly IHotelRepository _hotels;
    private readonly IRoomTypeRepository _types;
    private readonly IRateRepository _rates;

    public ChannelImportService(IBookingRepository bookings, IGuestRepository guests, IHotelRepository hotels,
        IRoomTypeRepository types, IRateRepository rates)
    {
        _bookings = bookings;
        _guests = guests;
        _hotels = hotels;
        _types = types;
        _rates = rates;
    }

    public async Task<ImportResultDto> ImportAsync(string csv)
    {
        var errors = new List<ImportRowError>();
        var imported = 0;
        var skipped = 0;
        var cancelled = 0;
        var total = 0;

        if (string.IsNullOrWhiteSpace(csv)) throw new BusinessRuleException("The file is empty.");
        var lines = csv.Replace("\r\n", "\n").Split('\n');
        var headerLine = lines[0];
        var header = new List<string>();
        var sb = new System.Text.StringBuilder();
        var inQuotes = false;
        foreach (var ch in headerLine)
        {
            if (ch == '"') { inQuotes = !inQuotes; continue; }
            if (ch == ',' && !inQuotes) { header.Add(sb.ToString().Trim().ToLowerInvariant()); sb.Clear(); continue; }
            sb.Append(ch);
        }
        header.Add(sb.ToString().Trim().ToLowerInvariant());
        var required = new[] { "external_ref", "channel", "hotel_code", "room_type_code", "guest_first", "guest_last",
            "guest_email", "check_in", "check_out", "adults", "status" };
        foreach (var r in required)
        {
            if (!header.Contains(r)) throw new BusinessRuleException($"Missing column '{r}'.");
        }

        for (var i = 1; i < lines.Length; i++)
        {
            var line = lines[i];
            if (string.IsNullOrWhiteSpace(line)) continue;
            total++;
            var cells = new List<string>();
            sb.Clear();
            inQuotes = false;
            for (var c = 0; c < line.Length; c++)
            {
                var ch = line[c];
                if (ch == '"')
                {
                    if (inQuotes && c + 1 < line.Length && line[c + 1] == '"') { sb.Append('"'); c++; continue; }
                    inQuotes = !inQuotes;
                    continue;
                }
                if (ch == ',' && !inQuotes) { cells.Add(sb.ToString().Trim()); sb.Clear(); continue; }
                sb.Append(ch);
            }
            cells.Add(sb.ToString().Trim());

            string Get(string name)
            {
                var idx = header.IndexOf(name);
                return idx >= 0 && idx < cells.Count ? cells[idx] : "";
            }

            var extRef = Get("external_ref");
            if (cells.Count < required.Length)
            {
                errors.Add(new ImportRowError(i + 1, extRef, "Row has too few columns."));
                skipped++;
                continue;
            }
            if (string.IsNullOrEmpty(extRef))
            {
                errors.Add(new ImportRowError(i + 1, extRef, "external_ref is required."));
                skipped++;
                continue;
            }

            var status = Get("status").ToUpperInvariant();
            var existing = await _bookings.GetByExternalRefAsync(extRef);

            if (status == "CANCELLED")
            {
                if (existing == null)
                {
                    errors.Add(new ImportRowError(i + 1, extRef, "Cancellation for a booking we do not hold."));
                    skipped++;
                    continue;
                }
                if (existing.Status == BookingStatuses.Cancelled)
                {
                    skipped++;
                    continue;
                }
                if (existing.Status != BookingStatuses.Confirmed)
                {
                    errors.Add(new ImportRowError(i + 1, extRef, $"Booking is {existing.Status} and cannot be cancelled."));
                    skipped++;
                    continue;
                }
                existing.Status = BookingStatuses.Cancelled;
                existing.RoomId = null;
                existing.UpdatedAt = DateTime.UtcNow;
                await _bookings.AddNoteAsync(new BookingNote
                {
                    BookingId = existing.Id,
                    Note = $"Cancelled by {existing.Channel} channel manager import.",
                    CreatedAt = DateTime.UtcNow
                });
                await _bookings.SaveAsync();
                cancelled++;
                imported++;
                continue;
            }

            if (!DateOnly.TryParseExact(Get("check_in"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var checkIn))
            {
                errors.Add(new ImportRowError(i + 1, extRef, "check_in is not a valid yyyy-MM-dd date."));
                skipped++;
                continue;
            }
            if (!DateOnly.TryParseExact(Get("check_out"), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var checkOut))
            {
                errors.Add(new ImportRowError(i + 1, extRef, "check_out is not a valid yyyy-MM-dd date."));
                skipped++;
                continue;
            }
            if (checkOut <= checkIn)
            {
                errors.Add(new ImportRowError(i + 1, extRef, "check_out must be after check_in."));
                skipped++;
                continue;
            }
            var nights = checkOut.DayNumber - checkIn.DayNumber;
            if (nights > 30)
            {
                errors.Add(new ImportRowError(i + 1, extRef, "Stay is longer than 30 nights."));
                skipped++;
                continue;
            }
            if (!int.TryParse(Get("adults"), out var adults) || adults < 1)
            {
                errors.Add(new ImportRowError(i + 1, extRef, "adults must be a whole number of at least 1."));
                skipped++;
                continue;
            }

            var hotel = await _hotels.GetByCodeAsync(Get("hotel_code").ToUpperInvariant());
            if (hotel == null)
            {
                errors.Add(new ImportRowError(i + 1, extRef, $"Unknown hotel code '{Get("hotel_code")}'."));
                skipped++;
                continue;
            }
            var type = await _types.GetByCodeAsync(Get("room_type_code").ToUpperInvariant());
            if (type == null)
            {
                errors.Add(new ImportRowError(i + 1, extRef, $"Unknown room type '{Get("room_type_code")}'."));
                skipped++;
                continue;
            }
            if (adults > type.MaxOccupancy)
            {
                errors.Add(new ImportRowError(i + 1, extRef, $"{type.Name} sleeps at most {type.MaxOccupancy}."));
                skipped++;
                continue;
            }

            var planCode = Get("rate_plan_code");
            if (string.IsNullOrEmpty(planCode)) planCode = "BAR";
            var plan = await _rates.GetPlanByCodeAsync(planCode.ToUpperInvariant());
            if (plan == null)
            {
                errors.Add(new ImportRowError(i + 1, extRef, $"Unknown rate plan '{planCode}'."));
                skipped++;
                continue;
            }

            var channel = Get("channel");
            if (channel.Equals("BKG", StringComparison.OrdinalIgnoreCase)) channel = "Booking.com";
            else if (channel.Equals("EXP", StringComparison.OrdinalIgnoreCase)) channel = "Expedia";
            else if (string.IsNullOrEmpty(channel)) channel = "Direct";

            decimal totalAmount;
            var amountText = Get("total_amount");
            if (!string.IsNullOrEmpty(amountText))
            {
                if (!decimal.TryParse(amountText, NumberStyles.Number, CultureInfo.InvariantCulture, out totalAmount) || totalAmount <= 0)
                {
                    errors.Add(new ImportRowError(i + 1, extRef, "total_amount is not a positive number."));
                    skipped++;
                    continue;
                }
            }
            else
            {
                var rate = await _rates.FindRateAsync(hotel.Id, type.Id, plan.Id, checkIn);
                if (rate == null)
                {
                    errors.Add(new ImportRowError(i + 1, extRef, "No rate loaded and no total_amount supplied."));
                    skipped++;
                    continue;
                }
                totalAmount = rate.NightlyRate * nights;
            }

            if (existing != null)
            {
                if (status == "MODIFIED" && existing.Status == BookingStatuses.Confirmed)
                {
                    existing.CheckIn = checkIn;
                    existing.CheckOut = checkOut;
                    existing.Adults = adults;
                    existing.TotalAmount = totalAmount;
                    existing.UpdatedAt = DateTime.UtcNow;
                    await _bookings.AddNoteAsync(new BookingNote
                    {
                        BookingId = existing.Id,
                        Note = $"Dates changed by {channel} channel manager import.",
                        CreatedAt = DateTime.UtcNow
                    });
                    await _bookings.SaveAsync();
                    imported++;
                }
                else
                {
                    skipped++;
                }
                continue;
            }

            var email = Get("guest_email");
            if (string.IsNullOrEmpty(email) || !email.Contains('@'))
            {
                errors.Add(new ImportRowError(i + 1, extRef, "guest_email is missing or not an email address."));
                skipped++;
                continue;
            }
            var guest = await _guests.GetByEmailAsync(email);
            if (guest == null)
            {
                var first = Get("guest_first");
                var last = Get("guest_last");
                if (string.IsNullOrEmpty(first) || string.IsNullOrEmpty(last))
                {
                    errors.Add(new ImportRowError(i + 1, extRef, "Guest name is required for a new guest."));
                    skipped++;
                    continue;
                }
                var phone = Get("guest_phone");
                guest = new Guest
                {
                    FirstName = first,
                    LastName = last,
                    Email = email,
                    Phone = string.IsNullOrEmpty(phone) ? "n/a" : phone,
                    AddressLine = "Not supplied by channel",
                    City = hotel.City,
                    Postcode = "n/a",
                    Country = "GB",
                    Region = hotel.Region,
                    LoyaltyTier = "None",
                    CreatedAt = DateTime.UtcNow
                };
                await _guests.AddAsync(guest);
            }

            var now = DateTime.UtcNow;
            var booking = new Booking
            {
                Reference = $"LH-{await _bookings.NextReferenceNumberAsync()}",
                ExternalRef = extRef,
                Channel = channel,
                HotelId = hotel.Id,
                GuestId = guest.Id,
                RoomTypeId = type.Id,
                RatePlanId = plan.Id,
                CheckIn = checkIn,
                CheckOut = checkOut,
                Adults = adults,
                Status = BookingStatuses.Confirmed,
                TotalAmount = totalAmount,
                CreatedAt = now,
                UpdatedAt = now
            };
            await _bookings.AddAsync(booking);
            await _bookings.AddNoteAsync(new BookingNote
            {
                BookingId = booking.Id,
                Note = $"Imported from {channel} ({extRef}).",
                CreatedAt = now
            });
            await _bookings.SaveAsync();
            imported++;
        }

        return new ImportResultDto(total, imported, skipped, cancelled, errors);
    }
}
