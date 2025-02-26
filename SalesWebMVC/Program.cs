﻿using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SalesWebMVC.Data;
using SalesWebMVC.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<SalesWebMVCContext>(options =>
//    options.UseSqlServer(builder.Configuration.GetConnectionString("SalesWebMVCContext")));

builder.Services.AddDbContext<SalesWebMVCContext>(options =>
       options.UseMySql(builder.Configuration.GetConnectionString("SalesWebMVCContext"),
       ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("SalesWebMVCContext")),
       b => b.MigrationsAssembly("SalesWebMVC")));

builder.Services.AddScoped<SeedingService>();
builder.Services.AddScoped<SellerService>();
builder.Services.AddScoped<DepartmentService>();
builder.Services.AddScoped<SalesRecordService>();
builder.Services.BuildServiceProvider();

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();
var enUS = new CultureInfo("en-US");
var localizationOptions = new RequestLocalizationOptions()
{
    DefaultRequestCulture = new RequestCulture(enUS),
    SupportedCultures = new List<CultureInfo> { enUS },
    SupportedUICultures = new List<CultureInfo> { enUS },
};
app.UseRequestLocalization(localizationOptions);
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    using var scope = app.Services.CreateScope();
    scope.ServiceProvider.GetService<SeedingService>().Seed();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
