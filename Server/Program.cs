using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using music_manager_starter.Data;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration) // Read from appsettings
    .CreateLogger();

// Add services to the container.
builder.Services.AddDbContext<DataDbContext>(options => options.UseSqlite(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

// Use Serilog
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

using (var scope = app.Services.CreateScope())
{
    var path = "App_Data";

    if (!Path.Exists(path)) Directory.CreateDirectory(path);
    var dbContext = scope.ServiceProvider.GetRequiredService<DataDbContext>();

    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();
