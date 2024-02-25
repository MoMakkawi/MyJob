using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using MyJob.Database;
using MyJob.EndPoints;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContext<MyJobContext>(options =>
    options
    .UseLazyLoadingProxies()
    .UseSqlServer(builder.Configuration.GetConnectionString("Context") ??
    throw new InvalidOperationException("Connection string 'Context' not found.")));

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "MyJob API",
        Description = "Minimal API as an example is an API for a website to get jobs",
        Contact = new OpenApiContact
        {
            Name = "MoMakkawi@Hotmail.com",
            Url = new Uri("https://linkedin.com/in/momakkawi/")
        }
    });
});

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







