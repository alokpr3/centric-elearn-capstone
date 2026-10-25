using System.Security.Cryptography;
using System.Text;
using FluentValidation;
using Harbourline.Api.Domain;
using Harbourline.Api.Dtos;
using Harbourline.Api.Infrastructure;
using Harbourline.Api.Mappers;
using Harbourline.Api.Repositories;

namespace Harbourline.Api.Services;

public interface IRegionService { Task<List<RegionDto>> ListAsync(); }

public class RegionService : IRegionService
{
    private readonly IRegionRepository _regions;
    public RegionService(IRegionRepository regions) => _regions = regions;
    public async Task<List<RegionDto>> ListAsync() => (await _regions.ListAsync()).Select(RegionMapper.ToDto).ToList();
}

public interface IProductService
{
    Task<List<ProductDto>> ListAsync(string? line, bool? active);
    Task<ProductDto> GetAsync(int id);
    Task<ProductDto> CreateAsync(ProductRequest request);
    Task<ProductDto> UpdateAsync(int id, ProductRequest request);
}

public class ProductService : IProductService
{
    private readonly IProductRepository _products;
    private readonly IValidator<ProductRequest> _validator;

    public ProductService(IProductRepository products, IValidator<ProductRequest> validator)
    {
        _products = products;
        _validator = validator;
    }

    public async Task<List<ProductDto>> ListAsync(string? line, bool? active) =>
        (await _products.ListAsync(line, active)).Select(ProductMapper.ToDto).ToList();

    public async Task<ProductDto> GetAsync(int id) =>
        ProductMapper.ToDto(await _products.GetByIdAsync(id) ?? throw new NotFoundException($"Product {id} was not found."));

    public async Task<ProductDto> CreateAsync(ProductRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        if (await _products.GetByCodeAsync(request.Code.Trim().ToUpperInvariant()) != null)
            throw new ConflictException($"Product code {request.Code} already exists.");
        var product = new Product();
        ProductMapper.Apply(product, request);
        return ProductMapper.ToDto(await _products.AddAsync(product));
    }

    public async Task<ProductDto> UpdateAsync(int id, ProductRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        var product = await _products.GetByIdAsync(id) ?? throw new NotFoundException($"Product {id} was not found.");
        ProductMapper.Apply(product, request);
        await _products.UpdateAsync(product);
        return ProductMapper.ToDto(product);
    }
}

public interface IUserService
{
    Task<List<UserDto>> ListAsync();
    Task<UserDto> GetAsync(int id);
    Task<UserDto> SetActiveAsync(int id, bool active);
    Task<LoginResponse> LoginAsync(LoginRequest request);
}

public class UserService : IUserService
{
    private readonly IUserRepository _users;
    private readonly IHandlerRepository _handlers;

    public UserService(IUserRepository users, IHandlerRepository handlers)
    {
        _users = users;
        _handlers = handlers;
    }

    public async Task<List<UserDto>> ListAsync() => (await _users.ListAsync()).Select(UserMapper.ToDto).ToList();

    public async Task<UserDto> GetAsync(int id) =>
        UserMapper.ToDto(await _users.GetByIdAsync(id) ?? throw new NotFoundException($"User {id} was not found."));

    public async Task<UserDto> SetActiveAsync(int id, bool active)
    {
        var user = await _users.GetByIdAsync(id) ?? throw new NotFoundException($"User {id} was not found.");
        user.Active = active;
        await _users.UpdateAsync(user);
        return UserMapper.ToDto(user);
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _users.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (user == null || !user.Active || user.PasswordHash != Hash(request.Password))
            throw new BusinessRuleException("Invalid email or password.");
        var handler = await _handlers.GetByUserIdAsync(user.Id);
        return new LoginResponse(user.Id, user.DisplayName, user.Role, handler?.Id);
    }

    public static string Hash(string password) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes("hl$" + password))).ToLowerInvariant();
}

public interface IHandlerService
{
    Task<List<HandlerDto>> ListAsync(int? regionId, bool? active);
    Task<HandlerDto> GetAsync(int id);
    Task<HandlerDto> CreateAsync(HandlerRequest request);
    Task<HandlerDto> UpdateAsync(int id, HandlerRequest request);
    Task<List<HandlerGradeDto>> ListGradesAsync();
    Task<CaseloadDto> GetCaseloadAsync(int id);
}

public class HandlerService : IHandlerService
{
    private readonly IHandlerRepository _handlers;
    private readonly IClaimService _claims;
    private readonly IValidator<HandlerRequest> _validator;

    public HandlerService(IHandlerRepository handlers, IClaimService claims, IValidator<HandlerRequest> validator)
    {
        _handlers = handlers;
        _claims = claims;
        _validator = validator;
    }

    public async Task<List<HandlerDto>> ListAsync(int? regionId, bool? active) =>
        (await _handlers.ListAsync(regionId, active)).Select(HandlerMapper.ToDto).ToList();

    public async Task<HandlerDto> GetAsync(int id) =>
        HandlerMapper.ToDto(await _handlers.GetByIdAsync(id) ?? throw new NotFoundException($"Handler {id} was not found."));

    public async Task<HandlerDto> CreateAsync(HandlerRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        var handler = new Handler();
        HandlerMapper.Apply(handler, request);
        await _handlers.AddAsync(handler);
        return await GetAsync(handler.Id);
    }

    public async Task<HandlerDto> UpdateAsync(int id, HandlerRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        var handler = await _handlers.GetByIdAsync(id) ?? throw new NotFoundException($"Handler {id} was not found.");
        handler.Grade = null;
        handler.Region = null;
        HandlerMapper.Apply(handler, request);
        await _handlers.UpdateAsync(handler);
        return await GetAsync(id);
    }

    public async Task<List<HandlerGradeDto>> ListGradesAsync() =>
        (await _handlers.ListGradesAsync()).Select(HandlerMapper.ToDto).ToList();

    public async Task<CaseloadDto> GetCaseloadAsync(int id)
    {
        var handler = await _handlers.GetByIdAsync(id) ?? throw new NotFoundException($"Handler {id} was not found.");
        var all = new List<ClaimListItemDto>();
        foreach (var status in ClaimStatuses.Open)
        {
            var page = await _claims.ListAsync(new ClaimFilter { HandlerId = id, Status = status, PageSize = 200 });
            all.AddRange(page.Items);
        }
        return new CaseloadDto(handler.Id, handler.Name, all.Count, handler.MaxOpenClaims, all);
    }
}

public interface IPolicyholderService
{
    Task<PagedResult<PolicyholderDto>> ListAsync(string? search, int? regionId, int page, int pageSize);
    Task<PolicyholderDto> GetAsync(int id);
    Task<PolicyholderDto> CreateAsync(PolicyholderRequest request);
    Task<PolicyholderDto> UpdateAsync(int id, PolicyholderRequest request);
}

public class PolicyholderService : IPolicyholderService
{
    private readonly IPolicyholderRepository _holders;
    private readonly IValidator<PolicyholderRequest> _validator;

    public PolicyholderService(IPolicyholderRepository holders, IValidator<PolicyholderRequest> validator)
    {
        _holders = holders;
        _validator = validator;
    }

    public async Task<PagedResult<PolicyholderDto>> ListAsync(string? search, int? regionId, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);
        var (items, total) = await _holders.ListAsync(search, regionId, page, pageSize);
        return new PagedResult<PolicyholderDto>(items.Select(PolicyholderMapper.ToDto).ToList(), total, page, pageSize);
    }

    public async Task<PolicyholderDto> GetAsync(int id) =>
        PolicyholderMapper.ToDto(await _holders.GetByIdAsync(id) ?? throw new NotFoundException($"Policyholder {id} was not found."));

    public async Task<PolicyholderDto> CreateAsync(PolicyholderRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        var holder = new Policyholder { CreatedAt = DateTime.UtcNow };
        PolicyholderMapper.Apply(holder, request);
        return PolicyholderMapper.ToDto(await _holders.AddAsync(holder));
    }

    public async Task<PolicyholderDto> UpdateAsync(int id, PolicyholderRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        var holder = await _holders.GetByIdAsync(id) ?? throw new NotFoundException($"Policyholder {id} was not found.");
        PolicyholderMapper.Apply(holder, request);
        await _holders.UpdateAsync(holder);
        return PolicyholderMapper.ToDto(holder);
    }
}

public interface IPolicyService
{
    Task<PagedResult<PolicyDto>> ListAsync(string? status, int? productId, int? regionId, int page, int pageSize);
    Task<List<PolicyDto>> ListForHolderAsync(int holderId);
    Task<PolicyDto> GetAsync(int id);
    Task<PolicyDto> CreateAsync(PolicyRequest request);
    Task<PolicyDto> UpdateAsync(int id, PolicyRequest request);
    Task<PolicyDto> CancelAsync(int id);
    Task<RenewalQuoteDto> GetRenewalQuoteAsync(int id);
}

public class PolicyService : IPolicyService
{
    private readonly IPolicyRepository _policies;
    private readonly IPolicyholderRepository _holders;
    private readonly IProductRepository _products;
    private readonly IValidator<PolicyRequest> _validator;

    public PolicyService(IPolicyRepository policies, IPolicyholderRepository holders, IProductRepository products, IValidator<PolicyRequest> validator)
    {
        _policies = policies;
        _holders = holders;
        _products = products;
        _validator = validator;
    }

    public async Task<PagedResult<PolicyDto>> ListAsync(string? status, int? productId, int? regionId, int page, int pageSize)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 200);
        var (items, total) = await _policies.ListAsync(status, productId, regionId, page, pageSize);
        return new PagedResult<PolicyDto>(items.Select(PolicyMapper.ToDto).ToList(), total, page, pageSize);
    }

    public async Task<List<PolicyDto>> ListForHolderAsync(int holderId) =>
        (await _policies.ListForHolderAsync(holderId)).Select(PolicyMapper.ToDto).ToList();

    public async Task<PolicyDto> GetAsync(int id) =>
        PolicyMapper.ToDto(await _policies.GetByIdAsync(id) ?? throw new NotFoundException($"Policy {id} was not found."));

    public async Task<PolicyDto> CreateAsync(PolicyRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        var holder = await _holders.GetByIdAsync(request.PolicyholderId) ?? throw new NotFoundException($"Policyholder {request.PolicyholderId} was not found.");
        var product = await _products.GetByIdAsync(request.ProductId) ?? throw new NotFoundException($"Product {request.ProductId} was not found.");
        if (!product.Active) throw new BusinessRuleException("Product is no longer sold.");

        var sequence = await _policies.CountAsync() + 100001;
        var policy = new Policy
        {
            PolicyNumber = $"HL{(product.Line == "Home" ? "H" : "M")}-{sequence}",
            PolicyholderId = holder.Id,
            ProductId = product.Id,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            AnnualPremium = request.AnnualPremium,
            SumInsured = request.SumInsured,
            NoClaimsYears = request.NoClaimsYears,
            Status = "Active",
            RegionId = holder.RegionId,
            CreatedAt = DateTime.UtcNow
        };
        await _policies.AddAsync(policy);
        return await GetAsync(policy.Id);
    }

    public async Task<PolicyDto> UpdateAsync(int id, PolicyRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        var policy = await _policies.GetByIdAsync(id) ?? throw new NotFoundException($"Policy {id} was not found.");
        if (policy.Status == "Cancelled") throw new BusinessRuleException("A cancelled policy cannot be changed.");
        policy.Policyholder = null;
        policy.Product = null;
        policy.StartDate = request.StartDate;
        policy.EndDate = request.EndDate;
        policy.AnnualPremium = request.AnnualPremium;
        policy.SumInsured = request.SumInsured;
        policy.NoClaimsYears = request.NoClaimsYears;
        await _policies.UpdateAsync(policy);
        return await GetAsync(id);
    }

    public async Task<PolicyDto> CancelAsync(int id)
    {
        var policy = await _policies.GetByIdAsync(id) ?? throw new NotFoundException($"Policy {id} was not found.");
        if (policy.Status == "Cancelled") throw new ConflictException("Policy is already cancelled.");
        policy.Policyholder = null;
        policy.Product = null;
        policy.Status = "Cancelled";
        await _policies.UpdateAsync(policy);
        return await GetAsync(id);
    }

    public async Task<RenewalQuoteDto> GetRenewalQuoteAsync(int id)
    {
        var policy = await _policies.GetByIdAsync(id) ?? throw new NotFoundException($"Policy {id} was not found.");
        var discount = PolicyPricing.NoClaimsDiscount(policy.NoClaimsYears);
        return new RenewalQuoteDto(policy.Id, policy.PolicyNumber, policy.AnnualPremium, discount * 100,
            PolicyPricing.RenewalPremium(policy.AnnualPremium, policy.NoClaimsYears), policy.EndDate.AddDays(1), policy.EndDate.AddYears(1));
    }
}

public interface IClaimService
{
    Task<PagedResult<ClaimListItemDto>> ListAsync(ClaimFilter filter);
    Task<ClaimDto> GetAsync(int id);
    Task<ClaimDto> CreateAsync(CreateClaimRequest request, int userId);
    Task<ClaimDto> UpdateAsync(int id, UpdateClaimRequest request);
    Task<ClaimDto> ChangeStatusAsync(int id, ChangeStatusRequest request);
    Task<ClaimDto> AssignAsync(int id, AssignClaimRequest request);
    Task<ClaimDto> ReopenAsync(int id, ReopenClaimRequest request);
    Task<List<ClaimHistoryDto>> GetHistoryAsync(int id);
}

public class ClaimService : IClaimService
{
    private readonly IClaimRepository _claims;
    private readonly IPolicyRepository _policies;
    private readonly IPolicyholderRepository _holders;
    private readonly IProductRepository _products;
    private readonly IHandlerRepository _handlers;
    private readonly IClaimNoteRepository _notes;
    private readonly IValidator<CreateClaimRequest> _createValidator;
    private readonly IValidator<UpdateClaimRequest> _updateValidator;
    private readonly IValidator<ChangeStatusRequest> _statusValidator;
    private readonly IValidator<ReopenClaimRequest> _reopenValidator;

    public ClaimService(IClaimRepository claims, IPolicyRepository policies, IPolicyholderRepository holders, IProductRepository products,
        IHandlerRepository handlers, IClaimNoteRepository notes, IValidator<CreateClaimRequest> createValidator,
        IValidator<UpdateClaimRequest> updateValidator, IValidator<ChangeStatusRequest> statusValidator, IValidator<ReopenClaimRequest> reopenValidator)
    {
        _claims = claims;
        _policies = policies;
        _holders = holders;
        _products = products;
        _handlers = handlers;
        _notes = notes;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _statusValidator = statusValidator;
        _reopenValidator = reopenValidator;
    }

    public async Task<PagedResult<ClaimListItemDto>> ListAsync(ClaimFilter filter)
    {
        filter.Page = Math.Max(filter.Page, 1);
        filter.PageSize = Math.Clamp(filter.PageSize, 1, 200);
        var claims = await _claims.ListAsync(filter);
        var total = await _claims.CountAsync(filter);

        var items = new List<ClaimListItemDto>();
        foreach (var claim in claims)
        {
            var policy = await _policies.GetByIdAsync(claim.PolicyId) ?? throw new NotFoundException($"Policy {claim.PolicyId} was not found.");
            var holder = await _holders.GetByIdAsync(policy.PolicyholderId) ?? throw new NotFoundException($"Policyholder {policy.PolicyholderId} was not found.");
            var product = await _products.GetByIdAsync(policy.ProductId) ?? throw new NotFoundException($"Product {policy.ProductId} was not found.");
            var handler = claim.HandlerId.HasValue ? await _handlers.GetByIdAsync(claim.HandlerId.Value) : null;
            var noteCount = await _notes.CountForClaimAsync(claim.Id);
            items.Add(ClaimMapper.ToListItem(claim, policy, holder, product, handler, noteCount));
        }
        return new PagedResult<ClaimListItemDto>(items, total, filter.Page, filter.PageSize);
    }

    public async Task<ClaimDto> GetAsync(int id)
    {
        var claim = await _claims.GetByIdAsync(id) ?? throw new NotFoundException($"Claim {id} was not found.");
        var lastNote = await _notes.LastNoteAtAsync(id);
        return ClaimMapper.ToDto(claim, lastNote.HasValue && ClaimRules.IsReserveReviewDue(lastNote.Value));
    }

    public async Task<ClaimDto> CreateAsync(CreateClaimRequest request, int userId)
    {
        await _createValidator.ValidateAndThrowAsync(request);
        var policy = await _policies.GetByIdAsync(request.PolicyId) ?? throw new NotFoundException($"Policy {request.PolicyId} was not found.");
        if (policy.Status == "Cancelled") throw new BusinessRuleException("Claims cannot be registered against a cancelled policy.");

        var now = DateTime.UtcNow;
        var sequence = await _claims.NextSequenceAsync();
        var claim = new Claim
        {
            ClaimNumber = ClaimRules.NextClaimNumber(sequence),
            PolicyId = policy.Id,
            ClaimType = request.ClaimType,
            Status = ClaimStatuses.New,
            LossDate = request.LossDate,
            ReportedDate = DateOnly.FromDateTime(now),
            Description = request.Description,
            EstimatedAmount = request.EstimatedAmount,
            Excess = policy.Product?.DefaultExcess ?? 0,
            RegionId = policy.RegionId,
            CreatedAt = now,
            UpdatedAt = now
        };
        await _claims.AddAsync(claim);
        await _claims.AddHistoryAsync(new ClaimStatusHistory
        {
            ClaimId = claim.Id, ToStatus = ClaimStatuses.New, ChangedByUserId = userId, ChangedAt = now, Reason = "Claim registered"
        });
        return await GetAsync(claim.Id);
    }

    public async Task<ClaimDto> UpdateAsync(int id, UpdateClaimRequest request)
    {
        await _updateValidator.ValidateAndThrowAsync(request);
        var claim = await _claims.GetByIdAsync(id) ?? throw new NotFoundException($"Claim {id} was not found.");
        claim.ClaimType = request.ClaimType;
        claim.Description = request.Description;
        claim.EstimatedAmount = request.EstimatedAmount;
        claim.ApprovedAmount = request.ApprovedAmount;
        claim.PaidAmount = request.PaidAmount;
        claim.UpdatedAt = DateTime.UtcNow;
        await _claims.UpdateAsync(claim);
        return await GetAsync(id);
    }

    public async Task<ClaimDto> ChangeStatusAsync(int id, ChangeStatusRequest request)
    {
        await _statusValidator.ValidateAndThrowAsync(request);
        var claim = await _claims.GetByIdAsync(id) ?? throw new NotFoundException($"Claim {id} was not found.");
        var from = claim.Status;
        var now = DateTime.UtcNow;

        claim.Status = request.Status;
        claim.UpdatedAt = now;
        if (request.Status == ClaimStatuses.Settled && claim.ApprovedAmount.HasValue)
            claim.PaidAmount = Math.Max(claim.ApprovedAmount.Value - claim.Excess, 0);
        if (request.Status == ClaimStatuses.Closed)
            claim.ClosedDate = DateOnly.FromDateTime(now);

        await _claims.UpdateAsync(claim);
        await _claims.AddHistoryAsync(new ClaimStatusHistory
        {
            ClaimId = id, FromStatus = from, ToStatus = request.Status, ChangedByUserId = request.ChangedByUserId, ChangedAt = now, Reason = request.Reason
        });
        return await GetAsync(id);
    }

    public async Task<ClaimDto> AssignAsync(int id, AssignClaimRequest request)
    {
        var claim = await _claims.GetByIdAsync(id) ?? throw new NotFoundException($"Claim {id} was not found.");
        var handler = await _handlers.GetByIdAsync(request.HandlerId) ?? throw new NotFoundException($"Handler {request.HandlerId} was not found.");
        if (!handler.Active) throw new BusinessRuleException("Handler is not active.");

        var openClaims = await _claims.CountOpenForHandlerAsync(handler.Id);
        if (openClaims >= handler.MaxOpenClaims)
            throw new ConflictException($"{handler.Name} already has {openClaims} open claims (limit {handler.MaxOpenClaims}).");

        claim.HandlerId = handler.Id;
        claim.UpdatedAt = DateTime.UtcNow;
        await _claims.UpdateAsync(claim);
        return await GetAsync(id);
    }

    public async Task<ClaimDto> ReopenAsync(int id, ReopenClaimRequest request)
    {
        await _reopenValidator.ValidateAndThrowAsync(request);
        var claim = await _claims.GetByIdAsync(id) ?? throw new NotFoundException($"Claim {id} was not found.");
        if (claim.Status != ClaimStatuses.Settled && claim.Status != ClaimStatuses.Closed)
            throw new BusinessRuleException("Only settled or closed claims can be reopened.");

        var from = claim.Status;
        var now = DateTime.UtcNow;
        claim.Status = ClaimStatuses.Reopened;
        claim.ReopenedCount += 1;
        claim.ClosedDate = null;
        claim.UpdatedAt = now;
        await _claims.UpdateAsync(claim);
        await _claims.AddHistoryAsync(new ClaimStatusHistory
        {
            ClaimId = id, FromStatus = from, ToStatus = ClaimStatuses.Reopened, ChangedByUserId = request.ChangedByUserId, ChangedAt = now, Reason = request.Reason
        });
        return await GetAsync(id);
    }

    public async Task<List<ClaimHistoryDto>> GetHistoryAsync(int id)
    {
        if (await _claims.GetByIdAsync(id) == null) throw new NotFoundException($"Claim {id} was not found.");
        return (await _claims.ListHistoryAsync(id)).Select(ClaimMapper.ToDto).ToList();
    }
}

public interface IClaimNoteService
{
    Task<List<ClaimNoteDto>> ListAsync(int claimId);
    Task<ClaimNoteDto> AddAsync(int claimId, CreateNoteRequest request);
    Task DeleteAsync(int noteId);
}

public class ClaimNoteService : IClaimNoteService
{
    private readonly IClaimNoteRepository _notes;
    private readonly IClaimRepository _claims;
    private readonly IValidator<CreateNoteRequest> _validator;

    public ClaimNoteService(IClaimNoteRepository notes, IClaimRepository claims, IValidator<CreateNoteRequest> validator)
    {
        _notes = notes;
        _claims = claims;
        _validator = validator;
    }

    public async Task<List<ClaimNoteDto>> ListAsync(int claimId)
    {
        if (await _claims.GetByIdAsync(claimId) == null) throw new NotFoundException($"Claim {claimId} was not found.");
        return (await _notes.ListForClaimAsync(claimId)).Select(ClaimMapper.ToDto).ToList();
    }

    public async Task<ClaimNoteDto> AddAsync(int claimId, CreateNoteRequest request)
    {
        await _validator.ValidateAndThrowAsync(request);
        if (await _claims.GetByIdAsync(claimId) == null) throw new NotFoundException($"Claim {claimId} was not found.");
        var note = new ClaimNote
        {
            ClaimId = claimId, AuthorUserId = request.AuthorUserId, NoteType = request.NoteType, Body = request.Body, CreatedAt = DateTime.UtcNow
        };
        return ClaimMapper.ToDto(await _notes.AddAsync(note));
    }

    public async Task DeleteAsync(int noteId)
    {
        var note = await _notes.GetByIdAsync(noteId) ?? throw new NotFoundException($"Note {noteId} was not found.");
        await _notes.DeleteAsync(note);
    }
}

public interface IReportService
{
    Task<List<LossRatioRow>> LossRatioAsync(DateOnly? from, DateOnly? to);
    Task<List<AgeingRow>> AgeingAsync();
    Task<List<HandlerWorkloadRow>> WorkloadAsync();
    Task<List<ClaimsSummaryRow>> SummaryAsync();
}

public class ReportService : IReportService
{
    private readonly IReportRepository _reports;
    private readonly IConfiguration _config;

    public ReportService(IReportRepository reports, IConfiguration config)
    {
        _reports = reports;
        _config = config;
    }

    public async Task<List<LossRatioRow>> LossRatioAsync(DateOnly? from, DateOnly? to)
    {
        var start = from ?? new DateOnly(2025, 10, 1);
        var end = to ?? new DateOnly(2026, 9, 30);
        if (end < start) throw new BusinessRuleException("'to' must not be before 'from'.");
        var rows = await _reports.LossRatioAsync(start.ToDateTime(TimeOnly.MinValue), end.AddDays(1).ToDateTime(TimeOnly.MinValue));
        return rows.Select(r => new LossRatioRow(r.ProductCode, r.ProductName, r.WrittenPremium, r.PaidClaims,
            LossRatioCalculator.Percent(r.PaidClaims, r.WrittenPremium), r.ClaimCount)).ToList();
    }

    public async Task<List<AgeingRow>> AgeingAsync()
    {
        var asOf = DateTime.Parse(_config["Reporting:AsOfDate"] ?? "2026-10-01").Date.AddDays(1);
        var rows = await _reports.AgeingAsync();
        return rows.GroupBy(r => r.Status).Select(g =>
        {
            var days = g.Select(r => (int)(asOf - (r.EnteredStatusAt ?? asOf)).TotalDays).ToList();
            return new AgeingRow(g.Key, g.Count(), Math.Round(days.Average(), 1), days.Max());
        }).OrderBy(r => Array.IndexOf(ClaimStatuses.All, r.Status)).ToList();
    }

    public async Task<List<HandlerWorkloadRow>> WorkloadAsync() =>
        (await _reports.WorkloadAsync()).Select(w => new HandlerWorkloadRow(w.HandlerId, w.HandlerName, w.GradeCode, w.OpenClaims, w.MaxOpenClaims, w.OpenEstimatedValue)).ToList();

    public async Task<List<ClaimsSummaryRow>> SummaryAsync() =>
        (await _reports.SummaryAsync()).Select(s => new ClaimsSummaryRow(s.RegionCode, s.Status, s.Claims, s.EstimatedTotal)).ToList();
}

public interface IImportBatchService
{
    Task<List<ImportBatchDto>> ListAsync();
    Task<ImportBatchDto> GetAsync(int id);
}

public class ImportBatchService : IImportBatchService
{
    private readonly IImportBatchRepository _batches;
    public ImportBatchService(IImportBatchRepository batches) => _batches = batches;
    public async Task<List<ImportBatchDto>> ListAsync() => (await _batches.ListAsync()).Select(ImportBatchMapper.ToDto).ToList();

    public async Task<ImportBatchDto> GetAsync(int id) =>
        ImportBatchMapper.ToDto(await _batches.GetByIdAsync(id) ?? throw new NotFoundException($"Import batch {id} was not found."));
}
