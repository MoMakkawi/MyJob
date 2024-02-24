using System;

using Microsoft.EntityFrameworkCore;

using MyJob.Database;
using MyJob.EndPoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<MyJobContext>(options =>
    options
    .UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("Context") ??
    throw new InvalidOperationException("Connection string 'Context' not found.")));

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseDeveloperExceptionPage();
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.Services
    .CreateScope()
    .ServiceProvider
    .GetRequiredService<MyJobContext>()
    .AdminSeeder();

app.MapFileDataEndpoints();
app.MapOpportunityEndpoints();
app.MapOpportunitySeekerEndpoints();
app.MapOrganizationEndpoints();
app.MapUserEndpoints();

app.Run();







