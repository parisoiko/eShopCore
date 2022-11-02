// Add services to the container.
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using eShopCore.Models;
using eShopCore.Hubs;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ProductDbContext")));

// Add services to the container.
var mvcBuilder = builder.Services.AddControllersWithViews();

mvcBuilder.AddRazorRuntimeCompilation();
builder.Services.AddSignalR();

builder.Services.AddTransient<ProductHub>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ProductHub>("/productHub");
app.Run();
