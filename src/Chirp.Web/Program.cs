using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using DotNetEnv;
using Azure.Security.KeyVault.Secrets;
using Azure.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddScoped<ICheepRepository, CheepRepository>();
builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();


// Connection string setup 
//Will move this in another cs file later, for more responsibility separation 
var kvUri = $"https://chirprazor.vault.azure.net/";
var client = new SecretClient(new Uri(kvUri), new DefaultAzureCredential());
var connectionString = String.Empty;
if (builder.Environment.IsDevelopment())
{
    KeyVaultSecret secret = client.GetSecret("azure-sql-test-connectionstring-test");
    connectionString = secret.Value;
}
else{
    KeyVaultSecret secret = client.GetSecret("azure-sql-connectionstring");
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

        context.Database.Migrate();
        // context.Database.EnsureCreated(); //Ensures all tables are created!
        // context.initializeDB();
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