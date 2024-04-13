using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using PeterDoStuff.Games;
using PeterDoStuff.MudWasmHosted.Client;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddMudServices();

builder.Services.AddScoped(_ => new GameOfLife(1, 1));
builder.Services.AddScoped(_ => MineSweeper.New(10, 10).RandomizeMines(10).Start());
builder.Services.AddScoped(_ => MatchFinder.New().Reset().AddRandoms(30));

await builder.Build().RunAsync();