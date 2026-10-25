using System.Globalization;
using System.Text;
using Northgate.Api.Domain;
using Northgate.Api.Dtos;
using Northgate.Api.Infrastructure;
using Northgate.Api.Models;
using Northgate.Api.Repositories;

namespace Northgate.Api.Services;

public class ManifestImportService
{
    private readonly IConsignmentRepository _consignments;
    private readonly ICustomerRepository _customers;
    private readonly IDepotRepository _depots;
    private readonly IImportBatchRepository _batches;

    public ManifestImportService(IConsignmentRepository consignments, ICustomerRepository customers, IDepotRepository depots, IImportBatchRepository batches)
    {
        _consignments = consignments; _customers = customers; _depots = depots; _batches = batches;
    }

    public async Task<List<ImportBatchDto>> ListBatchesAsync() =>
        (await _batches.ListAsync()).Select(b => new ImportBatchDto(b.Id, b.FileName, b.TotalRows, b.ImportedRows, b.SkippedRows, b.Summary, b.CreatedAt)).ToList();

    public async Task<ImportBatchDto> GetBatchAsync(int id)
    {
        var b = await _batches.GetAsync(id) ?? throw new NotFoundException($"Import batch {id} not found.");
        return new ImportBatchDto(b.Id, b.FileName, b.TotalRows, b.ImportedRows, b.SkippedRows, b.Summary, b.CreatedAt);
    }

    public async Task<ImportResultDto> ImportAsync(string fileName, Stream content)
    {
        var messages = new List<string>();
        var total = 0;
        var imported = 0;
        var skipped = 0;
        string text;
        using (var reader = new StreamReader(content, Encoding.UTF8))
        {
            text = await reader.ReadToEndAsync();
        }
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new BusinessRuleException("The manifest file is empty.");
        }
        var lines = text.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');
        var headerLine = lines[0].TrimStart('﻿');
        var headers = headerLine.Split(',').Select(h => h.Trim().ToLowerInvariant()).ToList();
        var required = new[] { "account_no", "origin_depot", "dest_depot", "service_level", "parcels", "weight_kg", "recipient_name", "delivery_address", "delivery_postcode" };
        foreach (var r in required)
        {
            if (!headers.Contains(r))
            {
                throw new BusinessRuleException($"Missing column '{r}'.");
            }
        }
        var seenReferences = new HashSet<string>();
        var knownCustomers = new Dictionary<string, Customer?>();
        var knownDepots = new Dictionary<string, Depot?>();
        for (var lineNo = 1; lineNo < lines.Length; lineNo++)
        {
            var line = lines[lineNo];
            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }
            total++;
            // split the line, honouring double quotes
            var cells = new List<string>();
            var sb = new StringBuilder();
            var inQuotes = false;
            foreach (var ch in line)
            {
                if (ch == '"')
                {
                    inQuotes = !inQuotes;
                }
                else if (ch == ',' && !inQuotes)
                {
                    cells.Add(sb.ToString());
                    sb.Clear();
                }
                else
                {
                    sb.Append(ch);
                }
            }
            cells.Add(sb.ToString());
            if (cells.Count != headers.Count)
            {
                messages.Add($"Line {lineNo + 1}: expected {headers.Count} columns but found {cells.Count}.");
                skipped++;
                continue;
            }
            string Cell(string name)
            {
                var idx = headers.IndexOf(name);
                return idx < 0 ? "" : cells[idx].Trim();
            }
            var accountNo = Cell("account_no").ToUpperInvariant();
            var originCode = Cell("origin_depot").ToUpperInvariant();
            var destCode = Cell("dest_depot").ToUpperInvariant();
            var level = Cell("service_level").ToUpperInvariant().Replace(' ', '_').Replace('-', '_');
            var parcelsText = Cell("parcels");
            var weightText = Cell("weight_kg");
            var recipient = Cell("recipient_name");
            var phone = Cell("recipient_phone");
            var address = Cell("delivery_address");
            var postcode = Cell("delivery_postcode").ToUpperInvariant();
            var reference = Cell("reference").ToUpperInvariant();
            var bookedText = Cell("booked_date");
            var noteText = Cell("notes");

            if (accountNo == "")
            {
                messages.Add($"Line {lineNo + 1}: account_no is blank.");
                skipped++;
                continue;
            }
            Customer? customer;
            if (!knownCustomers.TryGetValue(accountNo, out customer))
            {
                customer = await _customers.GetByAccountNoAsync(accountNo);
                knownCustomers[accountNo] = customer;
            }
            if (customer == null)
            {
                messages.Add($"Line {lineNo + 1}: unknown account {accountNo}.");
                skipped++;
                continue;
            }
            if (!customer.IsActive)
            {
                messages.Add($"Line {lineNo + 1}: account {accountNo} is not active.");
                skipped++;
                continue;
            }
            Depot? origin;
            if (!knownDepots.TryGetValue(originCode, out origin))
            {
                origin = await _depots.GetByCodeAsync(originCode);
                knownDepots[originCode] = origin;
            }
            if (origin == null)
            {
                messages.Add($"Line {lineNo + 1}: unknown origin depot {originCode}.");
                skipped++;
                continue;
            }
            Depot? dest;
            if (!knownDepots.TryGetValue(destCode, out dest))
            {
                dest = await _depots.GetByCodeAsync(destCode);
                knownDepots[destCode] = dest;
            }
            if (dest == null)
            {
                messages.Add($"Line {lineNo + 1}: unknown destination depot {destCode}.");
                skipped++;
                continue;
            }
            if (!ServiceLevels.All.Contains(level))
            {
                if (level == "NEXTDAY" || level == "NEXT")
                {
                    level = ServiceLevels.NextDay;
                }
                else if (level == "EXPRESS" || level == "AM")
                {
                    level = ServiceLevels.ExpressAm;
                }
                else if (level == "PALLET" || level == "48H")
                {
                    level = ServiceLevels.Pallet48;
                }
                else if (level == "")
                {
                    level = ServiceLevels.Economy;
                    messages.Add($"Line {lineNo + 1}: no service level given, defaulted to ECONOMY.");
                }
                else
                {
                    messages.Add($"Line {lineNo + 1}: unknown service level '{level}'.");
                    skipped++;
                    continue;
                }
            }
            if (!int.TryParse(parcelsText, out var parcels) || parcels < 1)
            {
                messages.Add($"Line {lineNo + 1}: parcels must be a whole number of at least 1.");
                skipped++;
                continue;
            }
            if (parcels > 500)
            {
                messages.Add($"Line {lineNo + 1}: parcels over 500 need a pallet booking.");
                skipped++;
                continue;
            }
            if (!decimal.TryParse(weightText, NumberStyles.Number, CultureInfo.InvariantCulture, out var weight) || weight <= 0)
            {
                messages.Add($"Line {lineNo + 1}: weight_kg must be a positive number.");
                skipped++;
                continue;
            }
            if (weight > 25000)
            {
                messages.Add($"Line {lineNo + 1}: weight over 25,000 kg is not accepted.");
                skipped++;
                continue;
            }
            if (recipient == "" || address == "" || postcode == "")
            {
                messages.Add($"Line {lineNo + 1}: recipient, address and postcode are all required.");
                skipped++;
                continue;
            }
            if (recipient.Length > 100)
            {
                recipient = recipient.Substring(0, 100);
            }
            if (address.Length > 200)
            {
                address = address.Substring(0, 200);
            }
            if (postcode.Length > 10)
            {
                messages.Add($"Line {lineNo + 1}: postcode '{postcode}' is too long.");
                skipped++;
                continue;
            }
            DateTime booked;
            if (bookedText == "")
            {
                booked = DateTime.Now;
            }
            else if (DateTime.TryParseExact(bookedText, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d1))
            {
                booked = d1.AddHours(9);
            }
            else if (DateTime.TryParseExact(bookedText, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out var d2))
            {
                booked = d2.AddHours(9);
            }
            else
            {
                messages.Add($"Line {lineNo + 1}: booked_date '{bookedText}' is not a date.");
                skipped++;
                continue;
            }
            if (reference == "")
            {
                reference = await _consignments.NextReferenceAsync();
                while (seenReferences.Contains(reference))
                {
                    reference = "NGF" + (long.Parse(reference.Substring(3)) + 1);
                }
            }
            else
            {
                if (!reference.StartsWith("NGF") || reference.Length > 16)
                {
                    messages.Add($"Line {lineNo + 1}: reference '{reference}' is not a Northgate reference.");
                    skipped++;
                    continue;
                }
                if (seenReferences.Contains(reference))
                {
                    messages.Add($"Line {lineNo + 1}: reference {reference} appears twice in this file.");
                    skipped++;
                    continue;
                }
                var existing = await _consignments.GetByReferenceAsync(reference);
                if (existing != null)
                {
                    messages.Add($"Line {lineNo + 1}: reference {reference} already exists.");
                    skipped++;
                    continue;
                }
            }
            seenReferences.Add(reference);

            var consignment = new Consignment
            {
                Reference = reference,
                CustomerId = customer.Id,
                OriginDepotId = origin.Id,
                DestDepotId = dest.Id,
                RegionCode = origin.RegionCode,
                ServiceLevel = level,
                Status = ConsignmentStatus.Booked,
                Parcels = parcels,
                WeightKg = weight,
                RecipientName = recipient,
                RecipientPhone = phone,
                DeliveryAddress = address,
                DeliveryPostcode = postcode,
                BookedAt = booked,
                PromisedBy = PromiseCalculator.Calculate(booked, level),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            try
            {
                await _consignments.AddAsync(consignment);
                await _consignments.SaveAsync();
                if (noteText != "")
                {
                    await _consignments.AddNoteAsync(new ConsignmentNote
                    {
                        ConsignmentId = consignment.Id,
                        AuthorUserId = 1,
                        NoteType = "NOTE",
                        Note = noteText.Length > 1000 ? noteText.Substring(0, 1000) : noteText,
                        CreatedAt = DateTime.UtcNow
                    });
                    await _consignments.SaveAsync();
                }
                imported++;
            }
            catch (Exception ex)
            {
                messages.Add($"Line {lineNo + 1}: could not be saved ({ex.GetType().Name}).");
                skipped++;
            }
        }

        var summary = $"{imported} of {total} rows imported" + (skipped > 0 ? $", {skipped} skipped." : ".");
        var batch = new ImportBatch
        {
            FileName = fileName.Length > 200 ? fileName.Substring(0, 200) : fileName,
            TotalRows = total,
            ImportedRows = imported,
            SkippedRows = skipped,
            Summary = summary,
            CreatedAt = DateTime.UtcNow
        };
        await _batches.AddAsync(batch);
        await _batches.SaveAsync();
        return new ImportResultDto(batch.Id, total, imported, skipped, messages);
    }
}
