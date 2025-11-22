namespace Web.Models
{

    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class EventoVisita
    {
        public TipoEventoVisita Tipo { get; set; }
        public decimal Latitud { get; set; }   // mejor decimal para que coincida con el REST
        public decimal Longitud { get; set; }
        public string? Notas { get; set; }
    }
}
