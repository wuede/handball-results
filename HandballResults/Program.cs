using HandballResults.Services;
using Microsoft.Extensions.Caching.Memory;
using System;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddSingleton<IConfiguration>(config);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddOutputCache();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IConfigurationService, AppSettingsConfigurationService>();
builder.Services.AddSingleton<ShvResultService>();
builder.Services.AddScoped<ResultServiceResolver>(serviceProvider => key =>
{
    if (string.Equals(CachedShvResultService.ServiceResolverKey, key, StringComparison.OrdinalIgnoreCase))
    {
        return new CachedShvResultService(serviceProvider.GetRequiredService<ShvResultService>(),
            serviceProvider.GetRequiredService<IMemoryCache>());
    }

    return serviceProvider.GetRequiredService<ShvResultService>();
});

builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConfiguration(config);
    logging.AddConsole();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapRazorPages();
app.UseOutputCache();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action}/{id:int?}",
    defaults: new { controller = "Games", action = "Results" }
);

app.Run();

public delegate IResultService ResultServiceResolver(string key);