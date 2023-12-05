using HandballResults.Services;
using Microsoft.Extensions.Caching.Memory;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddOutputCache();
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<IConfigurationService, AppSettingsConfigurationService>();
builder.Services.AddScoped<ResultServiceResolver>(serviceProvider => key =>
{
    if (string.Equals(CachedShvResultService.ServiceResolverKey, key, StringComparison.OrdinalIgnoreCase))
    {
        return new CachedShvResultService(serviceProvider.GetRequiredService<ShvResultService>(),
            serviceProvider.GetRequiredService<IMemoryCache>());
    }

    return new ShvResultService(serviceProvider.GetRequiredService<IConfigurationService>());
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

app.UseOutputCache();


app.Run();

public delegate IResultService ResultServiceResolver(string key);