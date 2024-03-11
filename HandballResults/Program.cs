using HandballResults.Services;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);

IConfiguration config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.AddSingleton(config);
builder.Services.AddLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConfiguration(config);
    logging.AddConsole();
});


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddOutputCache();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IConfigurationService, AppSettingsConfigurationService>();

// register HttpClient which ignores SSL certs
builder.Services.AddHttpClient(ShvResultService.HttpClientName, c => { }).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (m, c, ch, e) => true
});
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
    defaults: new { controller = "Home", action = "Index" }
);

app.Run();

public delegate IResultService ResultServiceResolver(string key);