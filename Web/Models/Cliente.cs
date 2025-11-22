namespace Web.Models
{
    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class Cliente
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string CorreoContacto { get; set; } = string.Empty;
        public string TelefonoContacto { get; set; } = string.Empty;
        public string UrlMapa { get; set; } = string.Empty;
    }

    public class CrearCliente
    {
        public string Nombre { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string CorreoContacto { get; set; } = string.Empty;
        public string TelefonoContacto { get; set; } = string.Empty;
        public string UrlMapa { get; set; } = string.Empty;
    }
}
