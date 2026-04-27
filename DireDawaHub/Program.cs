using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DireDawaHub.Data;
using DireDawaHub.Services;
using DireDawaHub.Middleware;
using Microsoft.AspNetCore.Localization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization();
builder.Services.AddSignalR();

// Enable Global Language Resource Paths
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Register the Automated Data Fetcher Background Service
builder.Services.AddSingleton<DireDawaHub.Services.SystemStateService>();
builder.Services.AddHostedService<DireDawaHub.Services.AgricultureDataFetcherService>();

// Register Content Scheduler Service
builder.Services.AddHostedService<DireDawaHub.Services.ContentSchedulerService>();

// Register Email Service
builder.Services.AddScoped<DireDawaHub.Services.EmailService>();

// Register Weather Service
builder.Services.AddHttpClient<DireDawaHub.Services.WeatherService>();
builder.Services.AddScoped<DireDawaHub.Services.WeatherService>();

// Register Version Tracking Service
builder.Services.AddScoped<DireDawaHub.Services.VersionTrackingService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=diredawa.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireLowercase = true;
    options.Password.RequiredLength = 8;
    
    // Two-Factor Authentication Settings
    options.SignIn.RequireConfirmedEmail = false;
    options.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

// Start Language Configuration
var supportedCultures = new[] { "en", "am", "om", "so" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture(supportedCultures[0])
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

app.UseRequestLocalization(localizationOptions);
// End Language Configuration

app.UseAuthentication();
app.UseAuthorization();

// Admin IP Whitelisting Middleware (restricts admin routes to Ethiopian IPs/VPN)
app.UseAdminIpWhitelist();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<DireDawaHub.Hubs.NotificationHub>("/notificationHub");

// ===== SEED DEFAULT ADMIN ACCCOUNT =====
using (var scope = app.Services.CreateScope())
{
    // Ensure database is created and migrations are applied before using Identity managers
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await db.Database.MigrateAsync();

    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // Create the Admin Role if it doesn't exist
    if (!await roleManager.RoleExistsAsync("Admin"))
    {
        await roleManager.CreateAsync(new IdentityRole("Admin"));
    }

    // Create the master Admin Account if it doesn't exist
    if (await userManager.FindByEmailAsync("fikaduabraham093@gmail.com") == null)
    {
        var adminUser = new IdentityUser 
        { 
            UserName = "fikaduabraham093@gmail.com", 
            Email = "fikaduabraham093@gmail.com", 
            EmailConfirmed = true 
        };
        // Using customized simple password per user request
        await userManager.CreateAsync(adminUser, "12345qwer");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.Run();
