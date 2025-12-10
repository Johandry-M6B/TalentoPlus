using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TalentoPlus.Core.Entities;
using TalentoPlus.Infrastructure.Data;
using TalentoPlus.Infrastructure.Repositories;
using TalentoPlus.Infrastructure.Services;
using TalentoPlus.Infrastructure.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentity<User, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();

// Register Repositories and Services
builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IExcelService, ExcelService>();

// Configure Kestrel to listen on port 8080
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(8080);
});

var app = builder.Build();

Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine("Starting TalentoPlus Web...");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

// Don't redirect to HTTPS in Docker (we're using HTTP on port 8080)
// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Seed Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        Console.WriteLine("Running database migrations...");
        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        
        context.Database.Migrate();
        Console.WriteLine("Migrations completed successfully.");
        
        Console.WriteLine("Seeding database...");
        await DbSeeder.SeedAsync(context, userManager, roleManager);
        Console.WriteLine("Database seeding completed.");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating or seeding the database.");
        Console.WriteLine($"ERROR: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        throw; // Re-throw to prevent app from starting with broken DB
    }
}

app.Run();