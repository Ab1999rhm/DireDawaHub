using DireDawaHub.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddViewLocalization();
builder.Services.AddSignalR();

// Enable Global Language Resource Paths
builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

// Register the Automated Data Fetcher Background Service
builder.Services.AddHostedService<DireDawaHub.Services.AgricultureDataFetcherService>();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=diredawa.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => 
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
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

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<DireDawaHub.Hubs.NotificationHub>("/notificationHub");

// ===== SEED DEFAULT ADMIN ACCCOUNT =====
using (var scope = app.Services.CreateScope())
{
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
