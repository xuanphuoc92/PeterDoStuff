using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.AspNetCore.Hosting.StaticWebAssets;
using Microsoft.AspNetCore.ResponseCompression;
using PeterDoStuff.Database;
using PeterDoStuff.MudWasmHosted.Server.Auth;
using SmartComponents.Inference.OpenAI;

var builder = WebApplication.CreateBuilder(args);

StaticWebAssetsLoader.UseStaticWebAssets(builder.Environment, builder.Configuration);

// Add services to the container.

builder.Services.AddSmartComponents()    
    .WithInferenceBackend<OpenAIInferenceBackend>();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Create the singleton MemoryDb
MemoryDb db = new MemoryDb();
builder.Services.AddSingleton(_ => db);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

// Make the APIs to only accept the API requests from its Web Assembly
var baseAddress = app.Configuration["URLS"]
    ?.Split(";")
    .FirstOrDefault(url => url.ToLower().StartsWith("https://"));

var referrerEV = Environment.GetEnvironmentVariable("Referrer");

if (baseAddress != null)
    app.UseMiddleware<ReferrerValidationMiddleware>(baseAddress);
else if (referrerEV != null) // Cuz Azure may make baseAddress null
    app.UseMiddleware<ReferrerValidationMiddleware>(referrerEV);

app.UseRouting();


app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();