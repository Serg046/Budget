using Budget.Client.Repositories;
using Budget.Client.Services;
using Budget.Components;
using Budget.Repositories;
using Budget.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();
builder.Services.AddSingleton<IMongoClient>(_ =>
    new MongoClient(builder.Configuration["MongoDb:ConnectionString"]));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IMongoClient>().GetDatabase(builder.Configuration["MongoDb:DatabaseName"]));
builder.Services.AddSingleton<ISettingsRepository, SettingsRepository>();
builder.Services.AddSingleton<TransactionRepository>();
builder.Services.AddSingleton<ITransactionRepository>(sp => sp.GetRequiredService<TransactionRepository>());
builder.Services.AddSingleton<ISyncStatusRepository, SyncStatusRepository>();
builder.Services.AddSingleton<SyncService>();
builder.Services.AddScoped<SyncStatusState>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);

app.UseAntiforgery();

app.MapGet("/api/transactions", async (DateOnly from, DateOnly to, ITransactionRepository repo) =>
    await repo.Get(from, to));
app.MapGet("/api/sync-status/last-data-update", async (ISyncStatusRepository repo) =>
    await repo.GetLastDataUpdate());
app.MapPost("/api/sync-status/validate", async ([FromBody] string password, ISyncStatusRepository repo) =>
    await repo.ValidatePassword(password));
app.MapPost("/api/sync-status/sync", async ([FromBody] string password, ISyncStatusRepository repo) =>
    await repo.Sync(password));

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Budget.Client._Imports).Assembly);

app.Run();
