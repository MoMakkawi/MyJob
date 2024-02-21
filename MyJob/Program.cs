using Microsoft.EntityFrameworkCore;

using MyJob.Database;
using MyJob.EndPoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<MyJobContext>(options =>
    options
    .UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("Context") ??
    throw new InvalidOperationException("Connection string 'Context' not found.")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapOpportunityEndpoints();

app.MapFileDataEndpoints();

app.MapOpportunitySeekerEndpoints();

app.MapOrganizationEndpoints();

app.Run();
