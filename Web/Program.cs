using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Web.Servicios;

var builder = WebApplication.CreateBuilder(args);

// -------------------------
// MVC + Session + ApiServicio
// -------------------------
builder.Services.AddControllersWithViews();

builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<ApiServicio>();

builder.Services.AddSession(o =>
{
    o.Cookie.Name = "mvp.session";
    o.IdleTimeout = TimeSpan.FromHours(8);
    o.Cookie.HttpOnly = true;
    o.Cookie.IsEssential = true;
});

// -------------------------
// AUTENTICACIÓN POR COOKIES
// -------------------------
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.Name = "mvp.auth";
        options.LoginPath = "/Cuenta/Login";
        options.LogoutPath = "/Cuenta/Logout";
        options.AccessDeniedPath = "/Cuenta/Login";
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization();

// -------------------------
// HttpClient (base del API desde appsettings)
// -------------------------
string baseApi = builder.Configuration["Api:BaseUrl"] ?? "http://localhost:5000";
if (!baseApi.EndsWith("/")) baseApi += "/";

builder.Services.AddHttpClient("api", c =>
{
    c.BaseAddress = new Uri(baseApi);
});

var app = builder.Build();

// -------------------------
// Pipeline
// -------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// app.UseHttpsRedirection(); // si solo local http, puedes dejarlo comentado
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

// Rutas de conveniencia para evitar 404 en / y /Home/Index
app.MapGet("/", ctx =>
{
    ctx.Response.Redirect("/Cuenta/Login");
    return Task.CompletedTask;
});
app.MapGet("/Home", ctx =>
{
    ctx.Response.Redirect("/Cuenta/Login");
    return Task.CompletedTask;
});
app.MapGet("/Home/Index", ctx =>
{
    ctx.Response.Redirect("/Cuenta/Login");
    return Task.CompletedTask;
});

// Ruta por defecto
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cuenta}/{action=Login}/{id?}");

app.Run();
