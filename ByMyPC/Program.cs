using ByMyPc.Postgresql;
using ByMyPc.Postgresql.Repository;
using ByMyPc.Postgresql.Repository.Intefaces;
using ByMyPC.Models.CpuModels;
using ByMyPC.Models.CpuModels.DTO;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

Log.Logger = new LoggerConfiguration()
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();


builder.Host.UseSerilog();

builder.Services.AddDbContext<PgContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("postgresMainDb") ?? throw new ArgumentNullException("Connection string is null"));
});


builder.Services.AddScoped<ICpuRepo, CpuRepo>();


builder.Services.AddAutoMapper(prf => {
    prf.AddProfile<CpuMappingProfile>();

} );


builder.Services.AddTransient<IValidator<DTOCpuCreateModel>, CpuCreateValidation>();
builder.Services.AddTransient<IValidator<DTOCpuUpdateModel>, CpuUpdateValidation>();
var app = builder.Build();




// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
