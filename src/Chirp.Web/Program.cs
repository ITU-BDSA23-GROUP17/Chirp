using Chirp.Core;
using Chirp.Infrastructure;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAdB2C"));
builder.Services.AddRazorPages()
    .AddMicrosoftIdentityUI();

builder.Services.AddScoped<ICheepRepository, CheepRepository>();

builder.Configuration.AddEnvironmentVariables();


// Connection string setup
DotNetEnv.Env.Load();
string connectionString = String.Empty;
if (builder.Environment.IsDevelopment())
{
var azureServer = DotNetEnv.Env.GetString("AZURE_SQL_SERVER");
var azureIntialCatalog = DotNetEnv.Env.GetString("AZURE_SQL_INITIAL_CATALOG");
var azureUser = DotNetEnv.Env.GetString("AZURE_SQL_USER");
var azurePassword = DotNetEnv.Env.GetString("AZURE_SQL_PASSWORD");
connectionString = "Server="+azureServer+";Initial Catalog="+azureIntialCatalog+";Persist Security Info=False;User ID="+azureUser+";Password="+azurePassword+";MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";
}
else
{
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is null or empty. Please check your configuration.");
}

// Make sure to register your DbContext here
builder.Services.AddDbContext<ChirpDBContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ChirpDBContext>();
        //003fd7fc-7841-4cd0-abae-98f088a22b8b
        var b = context.Database.CanConnect();
        Console.WriteLine(context.Cheeps.Count());
        Console.WriteLine(b);
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