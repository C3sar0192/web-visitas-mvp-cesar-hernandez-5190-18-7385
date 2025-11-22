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
    public class SupervisionController : Controller
    {
        private readonly ApiServicio _api;

        public SupervisionController(ApiServicio api)
        {
            _api = api;
        }

        // 1) Mis Técnicos (Supervisor)
        [Authorize(Roles = "Supervisor")]
        [HttpGet]
        public async Task<IActionResult> MisTecnicos()
        {
            try
            {
                string hoy = DateTime.UtcNow.ToString("yyyy-MM-dd");

                List<TecnicoResumen>? tecnicos =
                    await _api.GetSafeAsync<List<TecnicoResumen>>(
                        $"usuarios/mis-tecnicos-resumen?fecha={hoy}");

                if (tecnicos == null)
                {
                    tecnicos = new List<TecnicoResumen>();
                }

                return View(tecnicos);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo obtener la información de los técnicos. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado en la vista de técnicos." }
                );
            }
        }

        // 2) Reporte de visitas por técnico (Supervisor)
        [Authorize(Roles = "Supervisor")]
        [HttpGet]
        public async Task<IActionResult> ReporteVisitas(
            int? tecnicoId,
            DateTime? desde,
            DateTime? hasta)
        {
            try
            {
                string hoy = DateTime.UtcNow.ToString("yyyy-MM-dd");

                List<TecnicoResumen>? tecnicos =
                    await _api.GetSafeAsync<List<TecnicoResumen>>(
                        $"usuarios/mis-tecnicos-resumen?fecha={hoy}");

                if (tecnicos == null)
                {
                    tecnicos = new List<TecnicoResumen>();
                }

                List<VisitaReporte> visitas = new List<VisitaReporte>();

                if (tecnicoId.HasValue)
                {
                    string url = $"reportes/visitas-por-tecnico?tecnicoId={tecnicoId.Value}";

                    if (desde.HasValue)
                    {
                        url += $"&desde={desde.Value:yyyy-MM-dd}";
                    }

                    if (hasta.HasValue)
                    {
                        url += $"&hasta={hasta.Value:yyyy-MM-dd}";
                    }

                    List<VisitaReporte>? respuesta =
                        await _api.GetSafeAsync<List<VisitaReporte>>(url);

                    if (respuesta != null)
                    {
                        visitas = respuesta;
                    }
                }

                ViewBag.Tecnicos = tecnicos;
                ViewBag.TecnicoId = tecnicoId;
                ViewBag.Desde = desde?.ToString("yyyy-MM-dd") ?? string.Empty;
                ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd") ?? string.Empty;

                return View(visitas);
            }
            catch (HttpRequestException)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "No se pudo obtener el reporte de visitas. Intente nuevamente más tarde." }
                );
            }
            catch (Exception)
            {
                return RedirectToAction(
                    "General",
                    "Error",
                    new { mensaje = "Ha ocurrido un error inesperado en el reporte de visitas." }
                );
            }
        }
    }
}
