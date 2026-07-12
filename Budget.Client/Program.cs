using Budget.Client.Repositories;
using Budget.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped<ITransactionRepository, TransactionApiClient>();
builder.Services.AddScoped<ISyncStatusRepository, SyncStatusApiClient>();
builder.Services.AddScoped<IMerchantMappingRepository, MerchantMappingApiClient>();
builder.Services.AddScoped<IMerchantNameExclusionRepository, MerchantNameExclusionApiClient>();
builder.Services.AddScoped<SyncStatusState>();

await builder.Build().RunAsync();
