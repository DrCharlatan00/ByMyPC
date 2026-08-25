using ByMyPc.Postgresql;
using ByMyPc.Postgresql.Repository;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Hubs;
using ByMyPC.Middlewares;
using ByMyPC.Models.CpuModels;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Services.CpuService;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddSignalR();

#region Logging
Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();



builder.Host.ConfigureLogging(log => {
    log.AddSerilog();
#if !DEBUG
    log.SetMinimumLevel(LogLevel.Warning);
#endif
#if DEBUG
    log.SetMinimumLevel(LogLevel.Information);
#endif


})
    .UseSerilog();

#endregion

#region DB
builder.Services.AddDbContext<PgContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("postgresMainDb") ?? throw new ArgumentNullException("Connection string is null"));
});

builder.Services.AddHealthChecks().AddNpgSql(
        builder.Configuration.GetConnectionString("postgresMainDb")!,
        healthQuery: "SELECT 1;",
        name: "ByMyPC",
        tags: ["db","ready"]
        
    );
#endregion

builder.Services.AddScoped<ICpuRepo, CpuRepo>();
builder.Services.AddScoped<ICpuService, CpuService>();


#region Mappers and Validators
builder.Services.AddAutoMapper(prf => {
    prf.AddProfile<CpuMappingProfile>();

} );



builder.Services.AddTransient<IValidator<DTOCpuCreateModel>, CpuCreateValidation>();
builder.Services.AddTransient<IValidator<DTOCpuUpdateModel>, CpuUpdateValidation>();
#endregion

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiddleware<MiddlewareExceptions>();

app.MapControllers();

app.MapHub<CpuHub>("/cpu-hub");

app.Run();
