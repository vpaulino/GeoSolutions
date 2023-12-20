using CountryApi.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using System;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionSettings = new ConnectionSettings(new Uri(builder.Configuration.GetConnectionString("elasticSearchDb")));
var client = new ElasticClient(connectionSettings);


builder.Services.AddSingleton<AdministrativeAreasRepository>((serviceProvider)=> 
{
    var repository = new AdministrativeAreasRepository(client, ".\\GeoJSON\\gadm41_PRT_3.json", "portugal_parish", serviceProvider.GetRequiredService<ILogger<AdministrativeAreasRepository>>());
    repository.SetupAsync().ConfigureAwait(false).GetAwaiter();
    return repository;
});

var app = builder.Build();
 
app.UseSwagger();
app.UseSwaggerUI();
 

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
