namespace Web.Models
{

    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class TecnicoResumen
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        /// <summary>
        /// Total histórico de visitas asignadas a este técnico
        /// por el supervisor actual.
        /// Este valor viene del API (TecnicoResumenDto.CantidadVisitas).
        /// </summary>
        public int CantidadVisitas { get; set; }

        /// <summary>
        /// Cantidad de visitas asignadas para la fecha consultada
        /// (en nuestro caso, "hoy" porque así llamamos al API).
        /// Este valor viene del API (TecnicoResumenDto.CantidadVisitasHoy).
        /// </summary>
        public int CantidadVisitasHoy { get; set; }

        /// <summary>
        /// Propiedad de conveniencia para la vista.
        /// </summary>
        public bool TieneAsignacionHoy
        {
            get { return CantidadVisitasHoy > 0; }
        }
    }
}
