using DNTCaptcha.Core;
using Gigras.Software.Cyt.Repositories;
using Gigras.Software.Cyt.Services;
using Gigras.Software.Database;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console() // Optional: Log to console for debugging
    .WriteTo.File(
         Path.Combine(AppContext.BaseDirectory, "Logs", "blockchain-.txt"),
        rollingInterval: RollingInterval.Day, // Create a new file every day
        retainedFileCountLimit: 7 // Keep last 7 files
    )
    .CreateLogger();
builder.Host.UseSerilog();
// Add services to the container.

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set the session timeout
    options.Cookie.HttpOnly = true; // Make the session cookie HttpOnly
    options.Cookie.IsEssential = true; // Essential for GDPR compliance
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/sauth/Login";
        options.AccessDeniedPath = "/sauth/AccessDenied";
        options.SlidingExpiration = true;  // Ensure sliding expiration is enabled
        options.ExpireTimeSpan = TimeSpan.FromHours(1);  // Set default cookie expiration time
    });

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 104857600; // 100MB
});
builder.Services.AddDNTCaptcha(options =>
        options.UseCookieStorageProvider()      // Use cookie storage
               .ShowThousandsSeparators(false)  // Optional setting for formatting
               .WithEncryptionKey("CYT"));      // Encryption key (for security)

builder.Services.AddDbContext<CytContext>(options => options
    .UseSqlServer(builder.Configuration.GetConnectionString("CytConnectionString"))
    .EnableSensitiveDataLogging());

builder.Services.AddCytRepository();
builder.Services.AddCytServices();

builder.Services.AddControllersWithViews();

builder.Services.AddControllers()
            .AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
            });

//if (builder.Environment.IsDevelopment())
//{
builder.Services.AddRazorPages().AddRazorRuntimeCompilation();
//}

var app = builder.Build();

app.Use(async (context, next) =>
{
    try
    {
        Log.Information("Processing request for {Path}", context.Request.Path);
        await next();
    }
    catch (Exception ex)
    {
        Log.Error(ex, "Unhandled exception during request");
        throw;
    }
});

if (app.Environment.IsDevelopment())
{
    app.Use(async (context, next) =>
    {
        context.Response.Headers["Cache-Control"] = "no-store, no-cache, must-revalidate, proxy-revalidate";
        context.Response.Headers["Pragma"] = "no-cache";
        context.Response.Headers["Expires"] = "0";
        await next();
    });
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/sadmin/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    // Redirect the root path to /superadmin/index
    endpoints.MapGet("/", async context =>
    {
        context.Response.Redirect("/sadmin");
    });

    // Default controller route
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=sadmin}/{action=Index}/{id?}");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=sadmin}/{action=Index}/{id?}");

app.Run();