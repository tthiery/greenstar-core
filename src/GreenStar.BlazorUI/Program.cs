using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using GreenStar.BlazorUI;
using Microsoft.FluentUI.AspNetCore.Components;
using GreenStar.BlazorUI.Components;
using GreenStar.AppService.Actor;
using GreenStar.AppService.Setup;
using GreenStar.AppService.Turn;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddFluentUIComponents();
builder.Services.AddSingleton<MapService>();
builder.Services.AddSingleton<ICommandService, CommandDomainService>();
builder.Services.AddSingleton<ISetupService, SetupDomainService>();
builder.Services.AddSingleton<ITurnService, TurnDomainService>();

await builder.Build().RunAsync();
