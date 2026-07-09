using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Budget.Client.Repositories;
using Budget.Components;
using Budget.Repositories;
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
builder.Services.AddSingleton<ITransactionRepository, TransactionRepository>();
builder.Services.AddSingleton<ISyncStatusRepository, SyncStatusRepository>();

var app = builder.Build();

var settings = await app.Services.GetRequiredService<ISettingsRepository>().Get();
using var http = new HttpClient { BaseAddress = new Uri("https://api.enablebanking.com/") };
var privateKeyPath = app.Configuration["EnableBanking:PrivateKeyPath"]!;
var rsa = LoadPrivateKey(privateKeyPath);
SetAuthHeader();

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

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Budget.Client._Imports).Assembly);

app.Run();

void SetAuthHeader()
{
    var jwt = BuildJwt(settings.BankSession.ApplicationId, rsa);
    http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);
}

RSA LoadPrivateKey(string path)
{
    var pem = File.ReadAllText(path);
    var key = RSA.Create();
    key.ImportFromPem(pem);
    return key;
}

string BuildJwt(string appId, RSA key)
{
    var header = new { typ = "JWT", alg = "RS256", kid = appId };
    var now = DateTimeOffset.UtcNow;
    var body = new
    {
        iss = "enablebanking.com",
        aud = "api.enablebanking.com",
        iat = now.ToUnixTimeSeconds(),
        exp = now.AddHours(1).ToUnixTimeSeconds()
    };

    var headerJson = JsonSerializer.Serialize(header);
    var bodyJson = JsonSerializer.Serialize(body);
    var headerB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(headerJson));
    var bodyB64 = Base64UrlEncode(Encoding.UTF8.GetBytes(bodyJson));
    var signingInput = $"{headerB64}.{bodyB64}";

    var signature = key.SignData(Encoding.UTF8.GetBytes(signingInput), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    var signatureB64 = Base64UrlEncode(signature);

    return $"{signingInput}.{signatureB64}";
}

string Base64UrlEncode(byte[] bytes) =>
    Convert.ToBase64String(bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_');