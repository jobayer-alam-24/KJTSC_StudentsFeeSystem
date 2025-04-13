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
        options.UseSqlite(@"Data Source=D:\TSCDB\StudentDB.db"));
builder.Services.AddControllersWithViews();
var app = builder.Build();
// Configure the HTTP request pipeline.

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=List}/{id?}");

app.Run();
//dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
