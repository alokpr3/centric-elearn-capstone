using Harbourline.Api.Data;
using Harbourline.Api.Domain;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Harbourline.Tests;

public class ApiFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "harbourline-" + Guid.NewGuid();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<HarbourlineDbContext>(o => o.UseInMemoryDatabase(_dbName));

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<HarbourlineDbContext>();
            Seed(db);
        });
    }

    private static void Seed(HarbourlineDbContext db)
    {
        db.Regions.AddRange(new Region { Id = 1, Code = "NTH", Name = "Scotland & North" }, new Region { Id = 3, Code = "LSE", Name = "London & South East" });
        db.Products.AddRange(
            new Product { Id = 1, Code = "HOME-STD", Name = "Home Standard", Line = "Home", BasePremium = 520, DefaultExcess = 250, Active = true },
            new Product { Id = 2, Code = "MOT-COMP", Name = "Motor Comprehensive", Line = "Motor", BasePremium = 980, DefaultExcess = 300, Active = true });
        for (var i = 1; i <= 7; i++)
            db.Policyholders.Add(new Policyholder
            {
                Id = i, FullName = "Test Holder " + i, Email = $"holder{i}@example.com", Phone = "07700 900000", AddressLine = "1 High Street",
                City = "Leeds", Postcode = "LS1 1AA", DateOfBirth = new DateOnly(1980, 1, 1), RegionId = 1, CreatedAt = new DateTime(2025, 1, 1)
            });
        db.SaveChanges();
    }
}
