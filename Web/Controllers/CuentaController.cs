using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Web.Models;
using Web.Servicios;

namespace Web.Controllers
{
    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class CuentaController : Controller
    {
        private readonly ApiServicio _api;

        public CuentaController(ApiServicio api)
        {
            _api = api;
        }

        // LOGIN - GET
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            if (User?.Identity != null && User.Identity.IsAuthenticated)
            {
                string? rol = User.FindFirstValue(ClaimTypes.Role);

                if (rol == "Tecnico")
                {
                    return RedirectToAction("Hoy", "Visitas");
                }

                return RedirectToAction("Tablero", "Visitas");
            }

            return View();
        }

        // LOGIN - POST
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(PeticionLogin modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                RespuestaLogin? respuesta =
                    await _api.PostAsync<PeticionLogin, RespuestaLogin>(
                        "autenticacion/iniciar-sesion",
                        modelo
                    );

                if (respuesta == null || string.IsNullOrEmpty(respuesta.Token))
                {
                    ModelState.AddModelError(string.Empty, "Usuario o contraseña incorrectos.");
                    return View(modelo);
                }

                HttpContext.Session.SetString("token", respuesta.Token);
                HttpContext.Session.SetString("rol", respuesta.Rol);

                List<Claim> claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name,  respuesta.Nombre),
                    new Claim(ClaimTypes.Role,  respuesta.Rol),
                    new Claim("Token",          respuesta.Token)
                };

                ClaimsIdentity identidad = new ClaimsIdentity(
                    claims,
                    CookieAuthenticationDefaults.AuthenticationScheme
                );

                ClaimsPrincipal principal = new ClaimsPrincipal(identidad);

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    principal
                );

                return RedirigirPorRol(respuesta.Rol);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo contactar el servicio de autenticación. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado durante el inicio de sesión." }
                );
            }
        }

        // LOGOUT
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        // Redirección por rol
        private IActionResult RedirigirPorRol(string? rol)
        {
            string rolLower = (rol ?? string.Empty).ToLower();

            if (rolLower == "tecnico")
            {
                return RedirectToAction("Hoy", "Visitas");
            }

            if (rolLower == "supervisor")
            {
                return RedirectToAction("Tablero", "Visitas");
            }

            return RedirectToAction("Index", "Clientes");
        }
    }
}
