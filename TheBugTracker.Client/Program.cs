using MudBlazor.Services;
using TheBugTracker.Client.Services;
using TheBugTracker.Client.Interfaces;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddAuthenticationStateDeserialization();
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

builder.Services.AddScoped<IProjectDTOService, WASMProjectDTOService>();
builder.Services.AddScoped<ICompanyDTOService, WASMCompanyDTOService>();

await builder.Build().RunAsync();