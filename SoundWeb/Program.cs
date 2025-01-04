
using BLL;
using DAL.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using SoundWeb.CustomClasses;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration;
var constr = config.GetConnectionString("SqlConnection");

builder.Services.AddDbContext<SoundContext>(options =>
    options.UseSqlServer(constr));

builder.Services.AddAuthorization();

builder.Services.AddScoped<MediaService>();
builder.Services.AddScoped<MusicFinderService>();
builder.Services.AddHttpClient(); 

builder.Services.AddScoped<OpenAIService>(sp => new OpenAIService(
    sp.GetRequiredService<HttpClient>(),
    builder.Configuration["OpenAI:ApiKey"],
    sp.GetRequiredService<MusicFinderService>(),
    sp.GetRequiredService<SoundContext>()

));

// Add services to the container.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new LayoutByRoleAttribute());
    options.Filters.Add(new AuthorizeFilter());
});

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // Установка времени жизни сессии
    options.Cookie.HttpOnly = true; // Сессионные куки доступны только через HTTP-запросы
    options.Cookie.IsEssential = true; // Сессионные куки необходимы для работы приложения
});

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        // Set the LoginPath and LogoutPath to your controller actions
        options.LoginPath = "/Users/Login";
        options.LogoutPath = "/Users/Logout";

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = 403;
            context.Response.Redirect("/Home/AccessDenied");
            return Task.CompletedTask;
        };

    });



builder.Services.AddAuthorization();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Включение использования сессий

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();

