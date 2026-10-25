using System.Globalization;
using System.Text;
using Harbourline.Api.Domain;
using Harbourline.Api.Dtos;
using Harbourline.Api.Repositories;

namespace Harbourline.Api.Imports;

public interface IBordereauImportService
{
    Task<ImportResultDto> ImportAsync(Stream csv, string fileName, string source, int userId);
}

public class BordereauImportService : IBordereauImportService
{
    private readonly IPolicyRepository _policies;
    private readonly IPolicyholderRepository _holders;
    private readonly IClaimRepository _claims;
    private readonly IHandlerRepository _handlers;
    private readonly IImportBatchRepository _batches;

    public BordereauImportService(IPolicyRepository policies, IPolicyholderRepository holders, IClaimRepository claims,
        IHandlerRepository handlers, IImportBatchRepository batches)
    {
        _policies = policies;
        _holders = holders;
        _claims = claims;
        _handlers = handlers;
        _batches = batches;
    }

    public async Task<ImportResultDto> ImportAsync(Stream csv, string fileName, string source, int userId)
    {
        var errors = new List<ImportErrorDto>();
        int total = 0, created = 0, updated = 0, rejected = 0;
        var lines = new List<string>();
        using (var reader = new StreamReader(csv, Encoding.UTF8, true))
        {
            string? l;
            while ((l = await reader.ReadLineAsync()) != null)
            {
                if (l.Trim().Length == 0) continue;
                lines.Add(l);
            }
        }
        if (lines.Count == 0)
        {
            var empty = await _batches.AddAsync(new ImportBatch
            {
                FileName = fileName, Source = source, TotalRows = 0, CreatedRows = 0, UpdatedRows = 0, RejectedRows = 0,
                CreatedAt = DateTime.UtcNow, CreatedByUserId = userId
            });
            errors.Add(new ImportErrorDto(0, "File is empty."));
            return new ImportResultDto(empty.Id, 0, 0, 0, 0, errors);
        }

        var headerCells = new List<string>();
        var hb = new StringBuilder();
        var hq = false;
        foreach (var ch in lines[0].TrimStart('﻿'))
        {
            if (ch == '"') hq = !hq;
            else if (ch == ',' && !hq) { headerCells.Add(hb.ToString().Trim().ToLowerInvariant()); hb.Clear(); }
            else hb.Append(ch);
        }
        headerCells.Add(hb.ToString().Trim().ToLowerInvariant());
        int iRef = headerCells.IndexOf("externalref"), iPol = headerCells.IndexOf("policynumber"), iLoss = headerCells.IndexOf("lossdate");
        int iRep = headerCells.IndexOf("reporteddate"), iType = headerCells.IndexOf("claimtype"), iDesc = headerCells.IndexOf("description");
        int iAmt = headerCells.IndexOf("estimatedamount"), iMail = headerCells.IndexOf("contactemail");
        if (iRef < 0 || iPol < 0 || iLoss < 0 || iAmt < 0)
        {
            var bad = await _batches.AddAsync(new ImportBatch
            {
                FileName = fileName, Source = source, TotalRows = lines.Count - 1, CreatedRows = 0, UpdatedRows = 0, RejectedRows = lines.Count - 1,
                CreatedAt = DateTime.UtcNow, CreatedByUserId = userId
            });
            errors.Add(new ImportErrorDto(1, "Header must contain ExternalRef, PolicyNumber, LossDate and EstimatedAmount."));
            return new ImportResultDto(bad.Id, lines.Count - 1, 0, 0, lines.Count - 1, errors);
        }

        var seenRefs = new HashSet<string>();
        for (var i = 1; i < lines.Count; i++)
        {
            total++;
            var rowNo = i + 1;
            try
            {
                var cells = new List<string>();
                var sb = new StringBuilder();
                var inQ = false;
                var line = lines[i];
                for (var k = 0; k < line.Length; k++)
                {
                    var ch = line[k];
                    if (ch == '"')
                    {
                        if (inQ && k + 1 < line.Length && line[k + 1] == '"') { sb.Append('"'); k++; }
                        else inQ = !inQ;
                    }
                    else if (ch == ',' && !inQ) { cells.Add(sb.ToString().Trim()); sb.Clear(); }
                    else sb.Append(ch);
                }
                cells.Add(sb.ToString().Trim());
                string Cell(int idx) => idx >= 0 && idx < cells.Count ? cells[idx] : "";

                var extRef = Cell(iRef);
                var policyNumber = Cell(iPol).ToUpperInvariant();
                if (extRef == "") { rejected++; errors.Add(new ImportErrorDto(rowNo, "ExternalRef is missing.")); continue; }
                if (policyNumber == "") { rejected++; errors.Add(new ImportErrorDto(rowNo, "PolicyNumber is missing.")); continue; }
                if (!seenRefs.Add(extRef)) { rejected++; errors.Add(new ImportErrorDto(rowNo, "Duplicate ExternalRef in file.")); continue; }

                DateOnly lossDate;
                var lossText = Cell(iLoss);
                var formats = new[] { "yyyy-MM-dd", "dd/MM/yyyy", "d MMM yyyy", "dd-MM-yyyy" };
                if (!DateOnly.TryParseExact(lossText, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out lossDate))
                {
                    rejected++;
                    errors.Add(new ImportErrorDto(rowNo, $"LossDate '{lossText}' is not a recognised date."));
                    continue;
                }
                DateOnly reportedDate;
                var repText = Cell(iRep);
                if (repText == "") reportedDate = lossDate;
                else if (!DateOnly.TryParseExact(repText, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out reportedDate))
                {
                    rejected++;
                    errors.Add(new ImportErrorDto(rowNo, $"ReportedDate '{repText}' is not a recognised date."));
                    continue;
                }
                if (reportedDate < lossDate) { rejected++; errors.Add(new ImportErrorDto(rowNo, "ReportedDate is before LossDate.")); continue; }
                if (lossDate > new DateOnly(2026, 10, 1)) { rejected++; errors.Add(new ImportErrorDto(rowNo, "LossDate is in the future.")); continue; }

                var amtText = Cell(iAmt).Replace("£", "").Replace(",", "").Replace(" ", "");
                if (!decimal.TryParse(amtText, NumberStyles.Number, CultureInfo.InvariantCulture, out var amount) || amount <= 0)
                {
                    rejected++;
                    errors.Add(new ImportErrorDto(rowNo, $"EstimatedAmount '{Cell(iAmt)}' is not a positive amount."));
                    continue;
                }
                if (amount > 500000) { rejected++; errors.Add(new ImportErrorDto(rowNo, "EstimatedAmount exceeds the bordereau ceiling.")); continue; }

                var claimType = Cell(iType);
                if (claimType == "") claimType = "Unspecified";
                if (claimType.Length > 40) claimType = claimType.Substring(0, 40);
                var description = Cell(iDesc);
                if (description == "") description = "Imported from bordereau " + fileName;
                if (description.Length > 1000) description = description.Substring(0, 1000);

                var policy = await _policies.GetByNumberAsync(policyNumber);
                if (policy == null) { rejected++; errors.Add(new ImportErrorDto(rowNo, $"Policy {policyNumber} was not found.")); continue; }
                if (policy.Status == "Cancelled") { rejected++; errors.Add(new ImportErrorDto(rowNo, $"Policy {policyNumber} is cancelled.")); continue; }
                if (lossDate < policy.StartDate || lossDate > policy.EndDate)
                {
                    rejected++;
                    errors.Add(new ImportErrorDto(rowNo, $"LossDate {lossDate:yyyy-MM-dd} is outside the period of policy {policyNumber}."));
                    continue;
                }

                var email = Cell(iMail);
                if (email != "" && policy.Policyholder != null && !string.Equals(policy.Policyholder.Email, email, StringComparison.OrdinalIgnoreCase)
                    && email.Contains('@') && email.Length <= 120)
                {
                    var holder = await _holders.GetByIdAsync(policy.PolicyholderId);
                    if (holder != null)
                    {
                        holder.Email = email;
                        await _holders.UpdateAsync(holder);
                    }
                }

                var existing = await _claims.GetByExternalRefAsync(extRef);
                var now = DateTime.UtcNow;
                if (existing != null)
                {
                    if (existing.Status == ClaimStatuses.Closed || existing.Status == ClaimStatuses.Declined)
                    {
                        rejected++;
                        errors.Add(new ImportErrorDto(rowNo, $"Claim {existing.ClaimNumber} is {existing.Status} and cannot be updated by import."));
                        continue;
                    }
                    if (existing.PolicyId != policy.Id)
                    {
                        rejected++;
                        errors.Add(new ImportErrorDto(rowNo, $"ExternalRef {extRef} already belongs to a different policy."));
                        continue;
                    }
                    var changed = false;
                    if (existing.EstimatedAmount != amount)
                    {
                        existing.EstimatedAmount = amount;
                        changed = true;
                    }
                    if (existing.Description != description && description != "Imported from bordereau " + fileName)
                    {
                        existing.Description = description;
                        changed = true;
                    }
                    if (changed)
                    {
                        existing.UpdatedAt = now;
                        await _claims.UpdateAsync(existing);
                        updated++;
                    }
                    else
                    {
                        updated++;
                    }
                    continue;
                }

                Handler? chosen = null;
                var candidates = await _handlers.ListAsync(policy.RegionId, true);
                var bestLoad = int.MaxValue;
                foreach (var candidate in candidates)
                {
                    var load = await _claims.CountOpenForHandlerAsync(candidate.Id);
                    if (load >= candidate.MaxOpenClaims) continue;
                    if (candidate.Grade != null && candidate.Grade.ApprovalLimit < amount && candidates.Any(c => c.Grade != null && c.Grade.ApprovalLimit >= amount))
                        continue;
                    if (load < bestLoad)
                    {
                        bestLoad = load;
                        chosen = candidate;
                    }
                }

                var sequence = await _claims.NextSequenceAsync();
                var claim = new Claim
                {
                    ClaimNumber = Services.ClaimRules.NextClaimNumber(sequence),
                    PolicyId = policy.Id,
                    HandlerId = chosen?.Id,
                    ClaimType = claimType,
                    Status = ClaimStatuses.New,
                    LossDate = lossDate,
                    ReportedDate = reportedDate,
                    Description = description,
                    EstimatedAmount = amount,
                    Excess = policy.Product?.DefaultExcess ?? 0,
                    RegionId = policy.RegionId,
                    ExternalRef = extRef,
                    CreatedAt = now,
                    UpdatedAt = now
                };
                await _claims.AddAsync(claim);
                await _claims.AddHistoryAsync(new ClaimStatusHistory
                {
                    ClaimId = claim.Id, ToStatus = ClaimStatuses.New, ChangedByUserId = userId, ChangedAt = now,
                    Reason = "Imported from " + source
                });
                created++;
            }
            catch (Exception ex)
            {
                rejected++;
                errors.Add(new ImportErrorDto(rowNo, "Row could not be processed: " + ex.Message));
            }
        }

        var batch = await _batches.AddAsync(new ImportBatch
        {
            FileName = fileName, Source = source, TotalRows = total, CreatedRows = created, UpdatedRows = updated,
            RejectedRows = rejected, CreatedAt = DateTime.UtcNow, CreatedByUserId = userId
        });
        return new ImportResultDto(batch.Id, total, created, updated, rejected, errors);
    }
}
