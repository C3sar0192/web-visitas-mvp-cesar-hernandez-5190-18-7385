namespace Web.Models
{

    //CESAR EDUARDO HERNANDEZ ALVARADO
    //CARNET 5190-18-7385
    //PROYECTO DE SEMINARIO DE PRIBADO "ANALIS Y DESARROOLLO DE SISTEMAS"
    //SEMINARIO DE PRIVADOS DE ANTIGUA GUAMTEMALA
    //PROYECTO DE VISITAS TECNICAS DE SKYNET S.A
    public class UsuarioEditar
    {
        public int Id { get; set; }

        public string Nombre { get; set; } = string.Empty;

        public string Correo { get; set; } = string.Empty;

        // "Administrador", "Supervisor" o "Tecnico"
        public string Rol { get; set; } = string.Empty;

        // Para crear o cambiar contraseña (puede ir vacío al editar si no se quiere cambiar)
        public string? Contrasena { get; set; }

        // Supervisor al que pertenece el técnico (solo se usa si Rol = "Tecnico")
        public int? IdSupervisor { get; set; }
    }
}
