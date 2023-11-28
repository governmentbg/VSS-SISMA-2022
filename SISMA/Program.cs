using SISMA.Core.Constants;
using SISMA.Extensions;
using SISMA.ModelBinders;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

// За добавяне на контексти, използвайте extension метода!!!
builder.Services.AddApplicationDbContext(builder.Configuration);
builder.Configuration.AddJsonFile("hosting.json", true, true).AddEnvironmentVariables("ASPNETCORE_");

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddMvcOptions(options =>
    {
        options.ModelBinderProviders.Insert(0, new DateTimeModelBinderProvider(FormatConstant.NormalDateFormat));
        options.ModelBinderProviders.Insert(1, new DoubleModelBinderProvider());
        options.ModelBinderProviders.Insert(2, new DecimalModelBinderProvider());
    });

builder.AddApplicationAuthentication();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// За конфигуриране на Identity, използвайте extension метода!!! 
builder.Services.AddApplicationIdentity();

// За добавяне на услуги, използвайте extension метода!!!
builder.Services.AddApplicationServices();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
    app.UseMigrationsEndPoint();
}
app.Use((authContext, next) =>
{
    authContext.Request.Scheme = "https";
    return next();
});
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();