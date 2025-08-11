using Microsoft.EntityFrameworkCore;
using PetWeb.Data;
using PetWeb.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// EF Core DbContext
var connectionString = builder.Configuration.GetConnectionString("Default") ?? "Data Source=app.db";
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(connectionString);
});

// Image storage service selection
var provider = builder.Configuration["Storage:Provider"] ?? "Auto";
if (string.Equals(provider, "AzureBlob", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IImageStorageService, AzureBlobImageStorageService>();
}
else if (string.Equals(provider, "Local", StringComparison.OrdinalIgnoreCase))
{
    builder.Services.AddSingleton<IImageStorageService, LocalImageStorageService>();
}
else
{
    // Auto: try Azure first, otherwise fallback to Local
    try
    {
        // probe config quickly
        var azureConn = builder.Configuration["Storage:Azure:ConnectionString"];
        var serviceUrl = builder.Configuration["Storage:Azure:BlobServiceUrl"];
        if (!string.IsNullOrWhiteSpace(azureConn) || !string.IsNullOrWhiteSpace(serviceUrl))
        {
            builder.Services.AddSingleton<IImageStorageService, AzureBlobImageStorageService>();
        }
        else
        {
            builder.Services.AddSingleton<IImageStorageService, LocalImageStorageService>();
        }
    }
    catch
    {
        builder.Services.AddSingleton<IImageStorageService, LocalImageStorageService>();
    }
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Ensure database is created/migrated on startup (dev-friendly)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.Run();
