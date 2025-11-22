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

    [Authorize]
    public class VisitasController : Controller
    {
        private readonly ApiServicio _api;

        public VisitasController(ApiServicio api)
        {
            _api = api;
        }

        // Helpers: combos
        private async Task CargarCombosAsync()
        {
            List<UsuarioDto>? usuarios =
                await _api.GetSafeAsync<List<UsuarioDto>>("usuarios");

            if (usuarios == null)
            {
                usuarios = new List<UsuarioDto>();
            }

            List<UsuarioDto> tecnicos = new List<UsuarioDto>();
            foreach (UsuarioDto u in usuarios)
            {
                if (u.Rol == "Tecnico")
                {
                    tecnicos.Add(u);
                }
            }

            List<Cliente>? clientes =
                await _api.GetSafeAsync<List<Cliente>>("clientes");

            if (clientes == null)
            {
                clientes = new List<Cliente>();
            }

            ViewBag.Tecnicos = tecnicos;
            ViewBag.Clientes = clientes;
        }

        // TABLERO (Admin + Supervisor)
        [Authorize(Roles = "Administrador,Supervisor")]
        [HttpGet]
        public async Task<IActionResult> Tablero(int? tecnicoId, EstadoVisita? estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                // 1) Cargar técnicos para el combo
                List<UsuarioDto>? usuarios =
                    await _api.GetSafeAsync<List<UsuarioDto>>("usuarios");

                if (usuarios == null)
                {
                    usuarios = new List<UsuarioDto>();
                }

                List<UsuarioDto> tecnicos = new List<UsuarioDto>();
                foreach (UsuarioDto u in usuarios)
                {
                    if (u.Rol == "Tecnico")
                    {
                        tecnicos.Add(u);
                    }
                }

                ViewBag.Tecnicos = tecnicos;
                ViewBag.TecnicoId = tecnicoId;
                ViewBag.Estado = estado;
                ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd") ?? string.Empty;
                ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd") ?? string.Empty;

                // 2) Obtener todas las visitas desde el API
                List<Visita>? visitas =
                    await _api.GetSafeAsync<List<Visita>>("visitas");

                if (visitas == null)
                {
                    visitas = new List<Visita>();
                }

                // 3) Filtro por técnico
                if (tecnicoId.HasValue)
                {
                    UsuarioDto? tec = tecnicos.Find(t => t.Id == tecnicoId.Value);
                    if (tec != null)
                    {
                        string nombreTecnico = tec.Nombre;
                        visitas = visitas.FindAll(v =>
                            !string.IsNullOrWhiteSpace(v.NombreTecnico) &&
                            v.NombreTecnico.Equals(nombreTecnico, StringComparison.OrdinalIgnoreCase));
                    }
                }

                // 4) Filtro por estado
                if (estado.HasValue)
                {
                    visitas = visitas.FindAll(v => v.Estado == estado.Value);
                }

                // 5) Filtro por rango de fechas (comparando solo Date)
                if (fechaInicio.HasValue)
                {
                    DateTime fi = fechaInicio.Value.Date;
                    visitas = visitas.FindAll(v => v.FechaProgramada.Date >= fi);
                }

                if (fechaFin.HasValue)
                {
                    DateTime ff = fechaFin.Value.Date;
                    visitas = visitas.FindAll(v => v.FechaProgramada.Date <= ff);
                }

                // 6) Ordenar por fecha descendente
                visitas.Sort((a, b) => b.FechaProgramada.CompareTo(a.FechaProgramada));

                return View(visitas);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo obtener la información de visitas. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado en el tablero de visitas." }
                );
            }
        }

        // HOY (Técnico)
        [Authorize(Roles = "Tecnico")]
        [HttpGet]
        public async Task<IActionResult> Hoy()
        {
            try
            {
                List<Visita>? visitas =
                    await _api.GetSafeAsync<List<Visita>>("visitas/hoy");

                if (visitas == null)
                {
                    visitas = new List<Visita>();
                }

                return View(visitas);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo obtener sus visitas asignadas. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al cargar las visitas de hoy." }
                );
            }
        }

        // PLANIFICAR - GET (Admin + Supervisor)
        [Authorize(Roles = "Administrador,Supervisor")]
        [HttpGet]
        public async Task<IActionResult> Planificar()
        {
            try
            {
                await CargarCombosAsync();
                CrearVisita modelo = new CrearVisita();
                return View(modelo);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudieron cargar los datos necesarios para planificar la visita. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al preparar la planificación de visitas." }
                );
            }
        }

        // PLANIFICAR - POST
        [Authorize(Roles = "Administrador,Supervisor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Planificar(CrearVisita modelo)
        {
            if (!ModelState.IsValid)
            {
                await CargarCombosAsync();
                return View(modelo);
            }

            try
            {
                await _api.PostAsync("visitas", modelo);
                return RedirectToAction(nameof(Tablero));
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo registrar la visita. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al registrar la visita." }
                );
            }
        }

        // CHECK-IN de una visita (Técnico)
        [Authorize(Roles = "Tecnico")]
        [HttpPost]
        public async Task<IActionResult> CheckIn(long id)
        {
            try
            {
                EventoVisita evento = new EventoVisita
                {
                    Tipo = TipoEventoVisita.CheckIn,
                    Latitud = 0,
                    Longitud = 0,
                    Notas = string.Empty
                };

                await _api.PostAsync($"visitas/{id}/eventos", evento);

                return RedirectToAction(nameof(Hoy));
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo registrar el check-in de la visita. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al registrar el check-in." }
                );
            }
        }

        [Authorize(Roles = "Tecnico")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckOut(long id, string? notas)
        {
            try
            {
                EventoVisita ev = new EventoVisita
                {
                    Tipo = TipoEventoVisita.CheckOut,
                    Latitud = 0,
                    Longitud = 0,
                    Notas = notas
                };

                await _api.PostAsync($"visitas/{id}/eventos", ev);

                return RedirectToAction("Hoy");
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo registrar el cierre de la visita. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al registrar el cierre de la visita." }
                );
            }
        }

        [Authorize(Roles = "Tecnico")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancelar(long id, string? notas)
        {
            try
            {
                EventoVisita ev = new EventoVisita
                {
                    Tipo = TipoEventoVisita.Cancelacion,
                    Latitud = 0,
                    Longitud = 0,
                    Notas = notas
                };

                await _api.PostAsync($"visitas/{id}/eventos", ev);

                return RedirectToAction("Hoy");
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo registrar la cancelación de la visita. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado al registrar la cancelación de la visita." }
                );
            }
        }
    }
}
