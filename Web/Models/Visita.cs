using System;

namespace Web.Models
{
    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class Visita
    {
        public long Id { get; set; }

        public string NombreCliente { get; set; } = string.Empty;
        public string NombreSupervisor { get; set; } = string.Empty;
        public string NombreTecnico { get; set; } = string.Empty;

        public DateTime FechaProgramada { get; set; }
        public EstadoVisita Estado { get; set; }

        public string? UrlMapaCliente { get; set; }
        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }

        // Nota de planificación (la que se escribió al crear la visita)
        public string? NotasVisita { get; set; }

        // Nota del técnico (CheckOut / Cancelación)
        public string? NotasTecnico { get; set; }
    }
}
