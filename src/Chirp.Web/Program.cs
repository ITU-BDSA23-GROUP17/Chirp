using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

var azureAdB2COptions = builder.Configuration.GetSection("AzureADB2C");
// Connection string setup 
//Will move this in another cs file later, for more responsibility separation 
var kvUri = $"https://chirp-keys.vault.azure.net/";
var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
var connectionString = String.Empty;
KeyVaultSecret clientSecret = client.GetSecret("ClientSecret");

// Add the ClientSecret to the AzureADB2C configuration
azureAdB2COptions["ClientSecret"] = clientSecret.Value;


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
builder.Services.AddScoped<IFollowRepository, FollowRepository>();



if (builder.Environment.IsDevelopment())
{
    KeyVaultSecret secret = client.GetSecret("prod-connectionstring");
    connectionString = secret.Value;
}
else
{
    KeyVaultSecret secret = client.GetSecret("connectionstring");
    connectionString = secret.Value;
}


// Make sure to register your DbContext here
builder.Services.AddDbContext<ChirpDBContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ChirpDBContext>();

    // context.Database.Migrate();
    // context.Database.EnsureCreated(); //Ensures all tables are created!
    context.initializeDB();


    //context.Database.ExecuteSqlRaw("DROP TABLE [dbo].[__EFMigrationsHistory]"); // If a new table is added or column is added, uncomment this
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    builder.Configuration.AddEnvironmentVariables().AddJsonFile("appsettings.Development.json");
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();