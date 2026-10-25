using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Northgate.Api.Models;

namespace Northgate.Api.Data;

public class NorthgateDbContext : DbContext
{
    public NorthgateDbContext(DbContextOptions<NorthgateDbContext> options) : base(options) { }

    public DbSet<Depot> Depots => Set<Depot>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Driver> Drivers => Set<Driver>();
    public DbSet<Consignment> Consignments => Set<Consignment>();
    public DbSet<ConsignmentNote> ConsignmentNotes => Set<ConsignmentNote>();
    public DbSet<ConsignmentEvent> ConsignmentEvents => Set<ConsignmentEvent>();
    public DbSet<ImportBatch> ImportBatches => Set<ImportBatch>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Depot>().ToTable("depots");
        modelBuilder.Entity<Customer>().ToTable("customers");
        modelBuilder.Entity<AppUser>().ToTable("users");
        modelBuilder.Entity<Driver>().ToTable("drivers");
        modelBuilder.Entity<Consignment>().ToTable("consignments");
        modelBuilder.Entity<ConsignmentNote>().ToTable("consignment_notes");
        modelBuilder.Entity<ConsignmentEvent>().ToTable("consignment_events");
        modelBuilder.Entity<ImportBatch>().ToTable("import_batches");

        modelBuilder.Entity<Depot>(e =>
        {
            e.Property(x => x.Code).HasMaxLength(6);
            e.Property(x => x.Name).HasMaxLength(80);
            e.HasIndex(x => x.Code).IsUnique();
        });

        modelBuilder.Entity<Customer>(e =>
        {
            e.Property(x => x.AccountNo).HasMaxLength(12);
            e.Property(x => x.Name).HasMaxLength(120);
            e.Property(x => x.CreditLimit).HasPrecision(10, 2);
            e.HasIndex(x => x.AccountNo).IsUnique();
        });

        modelBuilder.Entity<Consignment>(e =>
        {
            e.Property(x => x.Reference).HasMaxLength(16);
            e.Property(x => x.ServiceLevel).HasMaxLength(16);
            e.Property(x => x.Status).HasMaxLength(20);
            e.Property(x => x.WeightKg).HasPrecision(8, 2);
            e.HasIndex(x => x.Reference).IsUnique();
            e.HasIndex(x => x.Status);
            e.HasIndex(x => x.PromisedBy);
            e.HasOne(x => x.Customer).WithMany().HasForeignKey(x => x.CustomerId);
            e.HasOne(x => x.OriginDepot).WithMany().HasForeignKey(x => x.OriginDepotId);
            e.HasOne(x => x.DestDepot).WithMany().HasForeignKey(x => x.DestDepotId);
            e.HasOne(x => x.Driver).WithMany().HasForeignKey(x => x.DriverId);
        });

        modelBuilder.Entity<ConsignmentNote>(e =>
        {
            e.Property(x => x.Note).HasMaxLength(1000);
            e.Property(x => x.NoteType).HasMaxLength(12);
        });

        modelBuilder.Entity<ConsignmentEvent>().Property(x => x.EventType).HasMaxLength(24);
        modelBuilder.Entity<AppUser>().HasIndex(x => x.Username).IsUnique();

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
            foreach (var property in entity.GetProperties())
                property.SetColumnName(ToSnake(property.Name));
    }

    private static string ToSnake(string name) =>
        Regex.Replace(name, "([a-z0-9])([A-Z])", "$1_$2").ToLowerInvariant();
}
