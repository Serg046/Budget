using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Budget.Client.Models;
using Budget.Client.Repositories;
using Budget.Client.Services;
using Budget.Components;
using Budget.Models;
using Budget.Repositories;
using Budget.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
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
builder.Services.AddSingleton<IMerchantMappingRepository, MerchantMappingRepository>();
builder.Services.AddSingleton<IMerchantNameExclusionRepository, MerchantNameExclusionRepository>();
builder.Services.AddSingleton<IMerchantGroupRepository, MerchantGroupRepository>();
builder.Services.AddSingleton<SyncService>();
builder.Services.AddScoped<SyncStatusState>();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.ExpireTimeSpan = TimeSpan.FromDays(365);
        options.SlidingExpiration = true;
        options.Events.OnValidatePrincipal = context =>
        {
            var username = context.Principal?.Identity?.Name;
            var stamp = context.Principal?.FindFirst("PasswordStamp")?.Value;
            var configuration = context.HttpContext.RequestServices.GetRequiredService<IConfiguration>();
            var user = configuration.GetSection("Auth:Users").Get<List<AuthUser>>()?
                .FirstOrDefault(u => u.Username == username);

            if (user is null || stamp != ComputePasswordStamp(user.Password))
            {
                context.RejectPrincipal();
            }

            return Task.CompletedTask;
        };
    });
builder.Services.AddAuthorization(options =>
    options.FallbackPolicy = new AuthorizationPolicyBuilder()
        .RequireAuthenticatedUser()
        .Build());

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

app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.MapGet("/api/transactions", async (DateOnly from, DateOnly to, ITransactionRepository repo) =>
    await repo.Get(from, to));
app.MapGet("/api/merchants", async (ITransactionRepository repo) =>
    await repo.GetDistinctMerchantNames());
app.MapGet("/api/merchant-totals", async (ITransactionRepository repo) =>
    await repo.GetTotalSpentByMerchant());
app.MapGet("/api/monthly-spend", async (DateOnly from, DateOnly to, ITransactionRepository repo) =>
    await repo.GetMonthlySpendByMerchant(from, to));
app.MapGet("/api/earliest-transaction-date", async (ITransactionRepository repo) =>
    await repo.GetEarliestBookingDate());
app.MapGet("/api/merchant-mappings", async (IMerchantMappingRepository repo) =>
    await repo.GetAll());
app.MapPost("/api/merchant-mappings", async (MerchantMapping mapping, IMerchantMappingRepository repo) =>
{
    try
    {
        await repo.SetMapping(mapping.MappedFrom, mapping.MappedTo);
        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
app.MapPost("/api/merchant-mappings/remove", async ([FromBody] string mappedFrom, IMerchantMappingRepository repo) =>
{
    await repo.RemoveMapping(mappedFrom);
    return Results.Ok();
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
app.MapGet("/api/merchant-name-exclusions", async (IMerchantNameExclusionRepository repo) =>
    await repo.GetAll());
app.MapPost("/api/merchant-name-exclusions", async ([FromBody] string word, IMerchantNameExclusionRepository repo) =>
{
    await repo.Add(word);
    return Results.Ok();
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
app.MapPost("/api/merchant-name-exclusions/remove", async ([FromBody] string word, IMerchantNameExclusionRepository repo) =>
{
    await repo.Remove(word);
    return Results.Ok();
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
app.MapGet("/api/merchant-groups", async (IMerchantGroupRepository repo) =>
    await repo.GetGroups());
app.MapPost("/api/merchant-groups", async ([FromBody] string name, IMerchantGroupRepository repo) =>
{
    try
    {
        await repo.AddGroup(name);
        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
app.MapPost("/api/merchant-groups/rename", async (RenameGroupRequest request, IMerchantGroupRepository repo) =>
{
    try
    {
        await repo.RenameGroup(request.OldName, request.NewName);
        return Results.Ok();
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
app.MapPost("/api/merchant-groups/remove", async ([FromBody] string name, IMerchantGroupRepository repo) =>
{
    await repo.RemoveGroup(name);
    return Results.Ok();
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
app.MapGet("/api/merchant-group-assignments", async (IMerchantGroupRepository repo) =>
    await repo.GetAssignments());
app.MapPost("/api/merchant-group-assignments", async (MerchantGroupAssignment assignment, IMerchantGroupRepository repo) =>
{
    await repo.SetAssignment(assignment.MerchantName, assignment.GroupName);
    return Results.Ok();
}).RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
app.MapGet("/api/sync-status/username", async (ISyncStatusRepository repo) =>
    await repo.GetUsername());
app.MapGet("/api/sync-status/is-admin", async (ISyncStatusRepository repo) =>
    await repo.IsAdmin());
app.MapGet("/api/sync-status/last-data-update", async (ISyncStatusRepository repo) =>
    await repo.GetLastDataUpdate());
app.MapGet("/api/sync-status/is-token-expiring-soon", async (ISyncStatusRepository repo) =>
    await repo.IsTokenExpiringSoon());
app.MapPost("/api/sync-status/sync", async (ISyncStatusRepository repo) =>
    await repo.Sync())
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });
app.MapPost("/api/sync-status/refresh-token", async (string? code, ISyncStatusRepository repo) =>
    Results.Json(await repo.RefreshToken(code)))
    .RequireAuthorization(new AuthorizeAttribute { Roles = "Admin" });

app.MapPost("/api/auth/login", async (HttpContext http, IConfiguration configuration) =>
{
    var form = await http.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    var user = configuration.GetSection("Auth:Users").Get<List<AuthUser>>()?
        .FirstOrDefault(u => u.Username == username && u.Password == password);

    if (user is null)
    {
        return Results.Redirect("/login?error=1");
    }

    var claims = new List<Claim>
    {
        new(ClaimTypes.Name, user.Username),
        new(ClaimTypes.Role, user.IsAdmin ? "Admin" : "User"),
        new("PasswordStamp", ComputePasswordStamp(user.Password))
    };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    await http.SignInAsync(
        CookieAuthenticationDefaults.AuthenticationScheme,
        new ClaimsPrincipal(identity),
        new AuthenticationProperties { IsPersistent = true });

    return Results.Redirect("/");
}).AllowAnonymous();

app.MapGet("/api/auth/logout", async (HttpContext http) =>
{
    await http.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/login");
}).AllowAnonymous();

app.MapStaticAssets().AllowAnonymous();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(Budget.Client._Imports).Assembly);

app.Run();

string ComputePasswordStamp(string password) =>
    Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(password)));
