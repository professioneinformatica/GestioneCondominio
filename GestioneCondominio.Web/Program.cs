using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GestioneCondominio.Web;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp =>
{
    var apiBaseUrl = builder.Configuration["services:api-service:https:0"]
        ?? builder.Configuration["services:api-service:http:0"]
        ?? builder.HostEnvironment.BaseAddress;

    return new HttpClient { BaseAddress = new Uri(apiBaseUrl) };
});

await builder.Build().RunAsync();
