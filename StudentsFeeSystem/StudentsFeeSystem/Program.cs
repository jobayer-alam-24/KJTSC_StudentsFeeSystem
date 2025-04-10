using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
using SQLitePCL;
using StudentsFeeSystem.Data;
Batteries.Init();
var builder = WebApplication.CreateBuilder(args);
QuestPDF.Settings.License = LicenseType.Community;
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite("Data Source=StudentDB.db"));
builder.Services.AddControllersWithViews();
var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
