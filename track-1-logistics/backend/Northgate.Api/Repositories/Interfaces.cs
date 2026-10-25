using Northgate.Api.Dtos;
using Northgate.Api.Models;

namespace Northgate.Api.Repositories;

public interface IDepotRepository
{
    Task<List<Depot>> ListAsync(string? regionCode);
    Task<Depot?> GetAsync(int id);
    Task<Depot?> GetByCodeAsync(string code);
    Task AddAsync(Depot depot);
    Task SaveAsync();
}

public interface ICustomerRepository
{
    Task<(List<Customer> Items, int Total)> SearchAsync(string? search, int page, int pageSize);
    Task<Customer?> GetAsync(int id);
    Task<Customer?> GetByAccountNoAsync(string accountNo);
    Task AddAsync(Customer customer);
    Task SaveAsync();
}

public interface IUserRepository
{
    Task<List<AppUser>> ListAsync();
    Task<AppUser?> GetAsync(int id);
    Task<AppUser?> GetByUsernameAsync(string username);
    Task SaveAsync();
}

public interface IDriverRepository
{
    Task<List<Driver>> ListAsync(int? depotId);
    Task<Driver?> GetAsync(int id);
    Task AddAsync(Driver driver);
    Task SaveAsync();
}

public interface IConsignmentRepository
{
    Task<(List<Consignment> Items, int Total)> SearchAsync(ConsignmentFilter filter);
    Task<Consignment?> GetAsync(int id);
    Task<Consignment?> GetByReferenceAsync(string reference);
    Task<List<Consignment>> ListByCustomerAsync(int customerId);
    Task<List<Consignment>> ListByDriverAsync(int driverId);
    Task<int> CountOpenForDriverAsync(int driverId);
    Task<List<Consignment>> ListPromisedBetweenAsync(DateTime from, DateTime toExclusive, int? depotId);
    Task<List<Consignment>> ListBookedBetweenAsync(DateTime from, DateTime toExclusive);
    Task<string> NextReferenceAsync();
    Task AddAsync(Consignment consignment);
    Task<List<ConsignmentNote>> ListNotesAsync(int consignmentId);
    Task AddNoteAsync(ConsignmentNote note);
    Task<List<ConsignmentEvent>> ListEventsAsync(int consignmentId);
    Task SaveAsync();
}

public interface IImportBatchRepository
{
    Task<List<ImportBatch>> ListAsync();
    Task<ImportBatch?> GetAsync(int id);
    Task AddAsync(ImportBatch batch);
    Task SaveAsync();
}
