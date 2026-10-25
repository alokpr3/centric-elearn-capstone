using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Northgate.Api.Data;
using Northgate.Api.Infrastructure;
using Northgate.Api.Repositories;
using Northgate.Api.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration["NORTHGATE_DB"]
    ?? builder.Configuration.GetConnectionString("Northgate")
    ?? throw new InvalidOperationException("No connection string configured.");

MigrationRunner.Run(connectionString);

builder.Services.AddDbContext<NorthgateDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4, 0))));

builder.Services.AddControllers().AddJsonOptions(o =>
    o.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IDepotRepository, DepotRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDriverRepository, DriverRepository>();
builder.Services.AddScoped<IConsignmentRepository, ConsignmentRepository>();
builder.Services.AddScoped<IImportBatchRepository, ImportBatchRepository>();

builder.Services.AddScoped<DepotService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<DriverService>();
builder.Services.AddScoped<ConsignmentService>();
builder.Services.AddScoped<ReportService>();
builder.Services.AddScoped<ManifestImportService>();

builder.Services.AddCors(o => o.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:5173").AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();

app.UseExceptionHandler();
app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

app.Run();

public partial class Program { }
