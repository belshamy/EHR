// F:\EHRsystem\Program.cs
using EHRsystem.Data;
using EHRsystem.Models.Entities; // <-- ADD THIS LINE for ApplicationUser
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore; // <-- ADD THIS LINE for AddDatabaseDeveloperPageExceptionFilter
using Microsoft.Extensions.Logging; // <-- ADD THIS LINE for ILogger
using Microsoft.AspNetCore.Identity.UI; // <-- ADD THIS LINE for UseMigrationsEndPoint (or ensure project references this)

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // Configure password policy if needed
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
})
.AddRoles<IdentityRole>() // <-- IMPORTANT: Add this if you use roles, which is highly likely for Admin/Doctor/Patient roles.
.AddEntityFrameworkStores<ApplicationDbContext>();

// If you have an EmailSender service that needs to be registered
// builder.Services.AddTransient<IEmailSender, EmailSender>(); // Uncomment and ensure IEmailSender/EmailSender exist

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages(); // Needed for Identity UI pages

var app = builder.Build();

// Apply migrations and seed database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        context.Database.Migrate();

        // Add seed data initialization here if needed
        // For example, to seed roles and admin user:
        // var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        // var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        // await SeedData.Initialize(context, userManager, roleManager); // Assuming you have a SeedData class
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// These two lines must be between UseRouting() and MapControllerRoute()/MapRazorPages()
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages(); // Needed for Identity UI pages

app.Run();