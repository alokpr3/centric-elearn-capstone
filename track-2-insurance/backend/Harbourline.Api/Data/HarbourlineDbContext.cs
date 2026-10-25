using System.Text;
using Harbourline.Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Harbourline.Api.Data;

public class HarbourlineDbContext : DbContext
{
    public HarbourlineDbContext(DbContextOptions<HarbourlineDbContext> options) : base(options) { }

    public DbSet<Region> Regions => Set<Region>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<HandlerGrade> HandlerGrades => Set<HandlerGrade>();
    public DbSet<AppUser> Users => Set<AppUser>();
    public DbSet<Handler> Handlers => Set<Handler>();
    public DbSet<Policyholder> Policyholders => Set<Policyholder>();
    public DbSet<Policy> Policies => Set<Policy>();
    public DbSet<Claim> Claims => Set<Claim>();
    public DbSet<ClaimNote> ClaimNotes => Set<ClaimNote>();
    public DbSet<ClaimStatusHistory> ClaimStatusHistory => Set<ClaimStatusHistory>();
    public DbSet<ImportBatch> ImportBatches => Set<ImportBatch>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        b.Entity<Region>().ToTable("regions");
        b.Entity<Product>().ToTable("products");
        b.Entity<HandlerGrade>().ToTable("handler_grades");
        b.Entity<AppUser>().ToTable("users");
        b.Entity<Handler>().ToTable("handlers");
        b.Entity<Policyholder>().ToTable("policyholders");
        b.Entity<Policy>().ToTable("policies");
        b.Entity<Claim>().ToTable("claims");
        b.Entity<ClaimNote>().ToTable("claim_notes");
        b.Entity<ClaimStatusHistory>().ToTable("claim_status_history");
        b.Entity<ImportBatch>().ToTable("import_batches");

        b.Entity<Region>().Property(x => x.Code).HasMaxLength(10);
        b.Entity<Product>().Property(x => x.Code).HasMaxLength(20);
        b.Entity<Product>().Property(x => x.Name).HasMaxLength(80);
        b.Entity<Product>().Property(x => x.Line).HasMaxLength(20);
        b.Entity<Policy>().Property(x => x.PolicyNumber).HasMaxLength(20);
        b.Entity<Policy>().HasIndex(x => x.PolicyNumber).IsUnique();
        b.Entity<Claim>().Property(x => x.ClaimNumber).HasMaxLength(20);
        b.Entity<Claim>().Property(x => x.Description).HasMaxLength(1000);
        b.Entity<Claim>().HasIndex(x => x.ClaimNumber).IsUnique();
        b.Entity<ClaimNote>().Property(x => x.Body).HasMaxLength(2000);
        b.Entity<ClaimStatusHistory>().Property(x => x.Reason).HasMaxLength(300);

        b.Entity<Claim>().HasOne(x => x.Policy).WithMany().HasForeignKey(x => x.PolicyId);
        b.Entity<Claim>().HasOne(x => x.Handler).WithMany().HasForeignKey(x => x.HandlerId);
        b.Entity<Policy>().HasOne(x => x.Policyholder).WithMany().HasForeignKey(x => x.PolicyholderId);
        b.Entity<Policy>().HasOne(x => x.Product).WithMany().HasForeignKey(x => x.ProductId);
        b.Entity<Handler>().HasOne(x => x.Grade).WithMany().HasForeignKey(x => x.GradeId);
        b.Entity<Handler>().HasOne(x => x.Region).WithMany().HasForeignKey(x => x.RegionId);

        foreach (var entity in b.Model.GetEntityTypes())
            foreach (var property in entity.GetProperties())
                property.SetColumnName(ToSnake(property.Name));
    }

    private static string ToSnake(string name)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < name.Length; i++)
        {
            if (char.IsUpper(name[i]) && i > 0) sb.Append('_');
            sb.Append(char.ToLowerInvariant(name[i]));
        }
        return sb.ToString();
    }
}
