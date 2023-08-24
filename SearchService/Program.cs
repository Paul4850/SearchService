using SearchAPI;
using System;
using Microsoft.Extensions.Configuration;
using System.Runtime.Intrinsics.Arm;
using Microsoft.Extensions.DependencyInjection;
using ProviderOne.ProviderOne;
using ProviderOne.ProviderTwo;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISearchService,  SearchService>();
builder.Services.Configure<List<SearchProviderSettings>>(builder.Configuration.GetSection("SearchProviderSettings"));

builder.Services.AddSingleton<ISearchProvider>(p =>
        new SearchProviderOne(builder.Configuration.GetSection("SearchProviderSettings")
        .Get<List<SearchProviderSettings>>().Where(s => s.Name == nameof(SearchProviderOne)).First().Uri
    ));

builder.Services.AddSingleton<ISearchProvider>(p =>
        new SearchProviderTwo(builder.Configuration.GetSection("SearchProviderSettings")
        .Get<List<SearchProviderSettings>>().Where(s => s.Name == nameof(SearchProviderTwo)).First().Uri
    ));


builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

app.Run();