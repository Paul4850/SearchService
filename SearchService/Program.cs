using SearchAPI;
using System;
using Microsoft.Extensions.Configuration;
using System.Runtime.Intrinsics.Arm;
using Microsoft.Extensions.DependencyInjection;
using ProviderOne.Provider1;
using ProviderOne.Provider2;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ISearchService,  SearchService>();
builder.Services.Configure<List<SearchProviderSettings>>(builder.Configuration.GetSection("SearchProviderSettings"));

builder.Services.AddSingleton<ISearchProvider>(p =>
        new SearchProvider1(builder.Configuration.GetSection("SearchProviderSettings")
        .Get<List<SearchProviderSettings>>().Where(s => s.Name == nameof(SearchProvider1)).First().Uri
    ));

builder.Services.AddSingleton<ISearchProvider>(p =>
        new SearchProvider2(builder.Configuration.GetSection("SearchProviderSettings")
        .Get<List<SearchProviderSettings>>().Where(s => s.Name == nameof(SearchProvider2)).First().Uri
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