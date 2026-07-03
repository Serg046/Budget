using Budget.Components;
using Budget.Repositories;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["MongoDb:ConnectionString"]));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(builder.Configuration["MongoDb:DatabaseName"]));
builder.Services.AddSingleton<ISettingsRepository, SettingsRepository>();

var app = builder.Build();

var settings = await app.Services.GetRequiredService<ISettingsRepository>().Get();
Console.WriteLine($"{settings.Id} BankSession: id={settings.BankSession.Id}, account={settings.BankSession.Account}, isActive={settings.BankSession.IsActive}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();