using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Models;

namespace Web.Controllers
{
    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A

    // Permitir que cualquier usuario vea la página de error
    [AllowAnonymous]
    public class ErrorController : Controller
    {
        // /Error/General?mensaje=...
        public IActionResult General(string? mensaje = null)
        {
            ErrorViewModel modelo = new ErrorViewModel
            {
                Titulo = "Ocurrió un error",
                Mensaje = string.IsNullOrWhiteSpace(mensaje)
                            ? "Ha ocurrido un error al procesar la solicitud. Por favor, inténtelo nuevamente."
                            : mensaje
            };

            return View("Error", modelo); // Busca Views/Error/Error.cshtml
        }
    }
}
