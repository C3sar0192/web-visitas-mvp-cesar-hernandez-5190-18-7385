using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

    [Authorize(Roles = "Administrador")]
    public class MantenimientoController : Controller
    {
        private readonly ApiServicio _api;

        public MantenimientoController(ApiServicio api)
        {
            _api = api;
        }

        // LISTA DE USUARIOS
        [HttpGet]
        public async Task<IActionResult> Usuarios()
        {
            try
            {
                List<UsuarioDto>? lista =
                    await _api.GetSafeAsync<List<UsuarioDto>>("usuarios");

                if (lista == null)
                {
                    lista = new List<UsuarioDto>();
                }

                return View(lista);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo obtener la lista de usuarios. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado en el módulo de usuarios." }
                );
            }
        }

        // CREAR USUARIO - GET
        [HttpGet]
        public async Task<IActionResult> CrearUsuario()
        {
            try
            {
                UsuarioEditar modelo = new UsuarioEditar();

                await CargarSupervisoresAsync();
                return View("UsuarioForm", modelo);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudieron cargar los supervisores. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al preparar la creación de usuario." }
                );
            }
        }

        // CREAR USUARIO - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario(UsuarioEditar modelo)
        {
            if (!ModelState.IsValid)
            {
                await CargarSupervisoresAsync();
                return View("UsuarioForm", modelo);
            }

            try
            {
                await _api.PostAsync("usuarios", modelo);
                return RedirectToAction(nameof(Usuarios));
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo crear el usuario. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al crear el usuario." }
                );
            }
        }

        // EDITAR USUARIO - GET
        [HttpGet]
        public async Task<IActionResult> EditarUsuario(int id)
        {
            try
            {
                UsuarioEditar? modelo =
                    await _api.GetSafeAsync<UsuarioEditar>($"usuarios/{id}");

                if (modelo == null)
                {
                    return NotFound();
                }

                await CargarSupervisoresAsync();
                return View("UsuarioForm", modelo);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo obtener la información del usuario. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al cargar el usuario." }
                );
            }
        }

        // EDITAR USUARIO - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarUsuario(int id, UsuarioEditar modelo)
        {
            if (id != modelo.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                await CargarSupervisoresAsync();
                return View("UsuarioForm", modelo);
            }

            try
            {
                await _api.PutAsync($"usuarios/{id}", modelo);
                return RedirectToAction(nameof(Usuarios));
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo actualizar el usuario. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al actualizar el usuario." }
                );
            }
        }

        // ELIMINAR USUARIO - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarUsuario(int id)
        {
            try
            {
                await _api.DeleteAsync($"usuarios/{id}");
                return RedirectToAction(nameof(Usuarios));
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo eliminar el usuario. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al eliminar el usuario." }
                );
            }
        }

        // Helper: carga lista de supervisores para el combo
        private async Task CargarSupervisoresAsync()
        {
            List<UsuarioDto>? usuarios =
                await _api.GetSafeAsync<List<UsuarioDto>>("usuarios");

            if (usuarios == null)
            {
                usuarios = new List<UsuarioDto>();
            }

            List<UsuarioDto> supervisores =
                usuarios.Where(u => u.Rol == "Supervisor").ToList();

            ViewBag.Supervisores = supervisores;
        }
    }
}
