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
var connectionString = EnvFileManager.GetConnectionString(builder.Environment.IsDevelopment());

// Make sure to register your DbContext here
builder.Services.AddDbContext<ChirpDBContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<ChirpDBContext>();
    //003fd7fc-7841-4cd0-abae-98f088a22b8b
    // var tableNames = context.Model.GetEntityTypes()
    //     .Select(t => t.GetTableName())
    //     .Distinct()
    //     .ToList();

    // foreach (var tableName in tableNames)
    // {
    //     context.Database.ExecuteSqlRaw($"TRUNCATE TABLE `{tableName}`;");
    // }
    context.Database.EnsureCreated();
    context.initializeDB();
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