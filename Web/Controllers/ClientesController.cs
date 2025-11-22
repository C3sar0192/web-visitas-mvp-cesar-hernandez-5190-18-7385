using System;
using System.Collections.Generic;
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

    [Authorize(Roles = "Administrador,Supervisor")]
    public class ClientesController : Controller
    {
        private readonly ApiServicio _api;

        public ClientesController(ApiServicio api)
        {
            _api = api;
        }

        // LISTA
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                List<Cliente>? clientes =
                    await _api.GetSafeAsync<List<Cliente>>("clientes");

                if (clientes == null)
                {
                    clientes = new List<Cliente>();
                }

                return View(clientes);  // @model List<Web.Models.Cliente>
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo obtener la lista de clientes. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado en el módulo de clientes." }
                );
            }
        }

        // CREAR - GET
        [HttpGet]
        public IActionResult Crear()
        {
            Cliente modelo = new Cliente();
            return View(modelo);    // @model Web.Models.Cliente
        }

        // CREAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Cliente modelo)
        {
            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                await _api.PostAsync("clientes", modelo);
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo crear el cliente. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al crear el cliente." }
                );
            }
        }

        // EDITAR - GET
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            try
            {
                Cliente? cliente =
                    await _api.GetSafeAsync<Cliente>($"clientes/{id}");

                if (cliente == null)
                {
                    return NotFound();
                }

                return View(cliente);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo obtener la información del cliente. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al cargar el cliente." }
                );
            }
        }

        // EDITAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Cliente modelo)
        {
            if (id != modelo.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(modelo);
            }

            try
            {
                await _api.PutAsync($"clientes/{id}", modelo);
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo actualizar el cliente. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al actualizar el cliente." }
                );
            }
        }

        // ELIMINAR - GET (confirmación)
        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {
            try
            {
                Cliente? cliente =
                    await _api.GetSafeAsync<Cliente>($"clientes/{id}");

                if (cliente == null)
                {
                    return NotFound();
                }

                return View(cliente);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo obtener la información del cliente a eliminar. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al cargar el cliente a eliminar." }
                );
            }
        }

        // ELIMINAR - POST
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                await _api.DeleteAsync($"clientes/{id}");
                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo eliminar el cliente. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al eliminar el cliente." }
                );
            }
        }
    }
}
