using Microsoft.AspNetCore.Builder;
using CryptoPriceTracker.Api.Data;
using CryptoPriceTracker.Api.Services;
using Microsoft.EntityFrameworkCore;
using CryptoPriceTracker.Api.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews().AddNewtonsoftJson();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=crypto.db"));
builder.Services.AddScoped<CryptoPriceService>();
builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ICryptoPriceService, CryptoPriceService>();


builder.Services.AddRazorPages();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.MapControllers();
app.Run();