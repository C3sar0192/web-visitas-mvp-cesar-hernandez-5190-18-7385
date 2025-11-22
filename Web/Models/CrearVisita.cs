using System.ComponentModel.DataAnnotations;

namespace Web.Models
{
    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class CrearVisita
    {
        [Required(ErrorMessage = "Seleccione un cliente.")]
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "Seleccione un técnico.")]
        public int IdTecnico { get; set; }

        // Valor que viene del <input type="datetime-local">, ej: 2025-11-05T08:00
        [Required(ErrorMessage = "La fecha programada es obligatoria.")]
        public string? FechaProgramada { get; set; }

        public string? Notas { get; set; }
    }
}
