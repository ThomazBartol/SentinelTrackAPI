using Microsoft.OpenApi.Models;
using SentinelTrack.Application.Mappings;
using SentinelTrack.Infrastructure.Context;
using SentinelTrack.Infrastructure.Repositories;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

Env.Load();

// Add services to the container.
builder.Services.AddAutoMapper(typeof(MotoMapping));
builder.Services.AddAutoMapper(typeof(YardMapping));

builder.Services.AddScoped<YardRepository>();
builder.Services.AddScoped<MotoRepository>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(swagger =>
    {
    swagger.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API do projeto SentinelTrack",
        Version = "v1",
        Description = "API do projeto SentinelTrack do Challenge da Mottu.",
        Contact = new OpenApiContact
        {
            Name = "Thomaz Bartol",
            Email = "rm555323@fiap.com.br"
        }
    });
});

var oracleConnectionString = Environment.GetEnvironmentVariable("ORACLE_CONNECTION_STRING");

// Configura o EF Core com Oracle
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseOracle(oracleConnectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
