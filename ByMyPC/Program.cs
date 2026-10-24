using ByMyPc.Postgresql;
using ByMyPc.Postgresql.Repository;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Caching;
using ByMyPC.Hubs;
using ByMyPC.Metrics;
using ByMyPC.Middlewares;
using ByMyPC.Models.CpuModels;
using ByMyPC.Models.CpuModels.DTO;
using ByMyPC.Models.GPUModels;
using ByMyPC.Models.GPUModels.DTO;
using ByMyPC.Models.HDDModels;
using ByMyPC.Models.HDDModels.DTO;
using ByMyPC.Models.MotherbordModels;
using ByMyPC.Models.MotherbordModels.DTO;
using ByMyPC.Models.PCModels;
using ByMyPC.Models.PCModels.DTO;
using ByMyPC.Models.PSUModels;
using ByMyPC.Models.PSUModels.DTO;
using ByMyPC.Models.RAMModels;
using ByMyPC.Models.RAMModels.DTO;
using ByMyPC.Services.CpuService;
using ByMyPC.Services.GPUService;
using ByMyPC.Services.HDDService;
using ByMyPC.Services.Kafka;
using ByMyPC.Services.MotherboardService;
using ByMyPC.Services.PCService;
using ByMyPC.Services.PSUService;
using ByMyPC.Services.RamService;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using Scalar.AspNetCore;
using Serilog;
using StackExchange.Redis;

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

builder.Services.AddStackExchangeRedisCache(opt =>
{
    opt.InstanceName = "ByMyPcCache";
    opt.Configuration = builder.Configuration.GetConnectionString("RedisMain");
});

builder.Services.AddSingleton<IConnectionMultiplexer>(
        ConnectionMultiplexer.Connect(builder.Configuration.GetConnectionString("RedisMain")!)
    );

builder.Services.AddHealthChecks().AddNpgSql(
        builder.Configuration.GetConnectionString("postgresMainDb")!,
        healthQuery: "SELECT 1;",
        name: "ByMyPC",
        tags: ["db", "ready"]

    ).AddRedis(
        builder.Configuration.GetConnectionString("RedisMain")!,
        name: "Redis",
        tags: ["cache","ready"]
    );


builder.Services.AddScoped<ICacheService, RedisCacheService>();
#endregion

builder.Services.AddScoped<ICpuRepo, CpuRepo>();
builder.Services.AddScoped<ICpuService, CpuService>();

builder.Services.AddScoped<IMotherboardRepo, MotherboardRepo>();
builder.Services.AddScoped<IMotherboardService, MotherboardService>();

builder.Services.AddScoped<IHddRepo,HddRepo>();
builder.Services.AddScoped<IPcHddRepo, PcHddRepo>();
builder.Services.AddScoped<IHDDService, HDDService>();

builder.Services.AddScoped<IPSURepo, PSURepo>();
builder.Services.AddScoped<IPSUService, PSUService>();

builder.Services.AddScoped<IGPURepo, GPURepo>();
builder.Services.AddScoped<IGPUService, GPUService>();

builder.Services.AddScoped<IRamRepo, RamRepo>();
builder.Services.AddScoped<IPcRamRepo, PcRamRepo>();
builder.Services.AddScoped<IRamService, RamService>();

builder.Services.AddScoped<IPcRepo, PcRepo>();
builder.Services.AddScoped<IPcService, PcService>();


builder.Services.AddSingleton<KafkaProducerService>();

#region Metrics

builder.Services.AddSingleton<DependencyHealthMetrics>();

builder.Services.AddHostedService(sp => sp.GetRequiredService<DependencyHealthMetrics>());

builder.Services.AddOpenTelemetry()
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddMeter("ByMyPc.Health")
        .AddPrometheusExporter());
#endregion

#region Mappers and Validators
builder.Services.AddAutoMapper(prf => {
    prf.AddProfile<CpuMappingProfile>();
    prf.AddProfile<MotherboardMappingClass>();
    prf.AddProfile<HDDMappingClass>();
    prf.AddProfile<PSUMappingModel>();
    prf.AddProfile<MappingGPUClass>();
    prf.AddProfile<MappingRamClass>();

} );



builder.Services.AddTransient<IValidator<DTOCpuCreateModel>, CpuCreateValidation>();
builder.Services.AddTransient<IValidator<DTOCpuUpdateModel>, CpuUpdateValidation>();

builder.Services.AddTransient<IValidator<DTOMotherboardCreateModel>, MotherboardCreateValidator>();
builder.Services.AddTransient<IValidator<DTOMotherboardUpdateModel>, MotherboardUpdateValidator>();

builder.Services.AddTransient<IValidator<DTOHDDCreateModel>, ValidatorHDDCreate>();

builder.Services.AddTransient<IValidator<DTOPSUModelCreate>, PSUValidator>();

builder.Services.AddTransient<IValidator<DTOGPUCreateModel>, GPUCreateValidator>();

builder.Services.AddTransient<IValidator<DTORAMCreate>, ValidatorRAMCreate>();

builder.Services.AddTransient<IValidator<DTOPCCreateModel>, PCValidatorCreate>();
#endregion

var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.UseWhen(
    context => context.Request.Path != "/metrics",
    branche => branche.UseHttpsRedirection()
    );

app.UseAuthorization();

app.UseMiddleware<MiddlewareExceptions>();

app.MapControllers();
#region Hubs 
app.MapHub<CpuHub>("/cpu-hub");
app.MapHub<MotherboardHub>("/motherboard-hub");
app.MapHub<HDDHub>("/hdd-hub");
app.MapHub<PSUHub>("/psu-hub");
app.MapHub<GPUHub>("/gpu-hub");
app.MapHub<RamHub>("/ram-hub");
app.MapHub<PcHub>("/pc-hub");
#endregion

app.MapPrometheusScrapingEndpoint();

#region Healths
app.MapHealthChecks("/health");

app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready")
});

app.MapHealthChecks("/health/redis", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Name == "cache"
});

app.MapHealthChecks("/health/db", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    Predicate = check => check.Name == "db"
});
#endregion
 app.Run();
