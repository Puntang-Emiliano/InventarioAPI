namespace InventarioAPI.ModelsDTO
{
    public class InicioSesionDTO
    {
        public int IdUsuario { get; set; }
        public string email { get; set; }
        public string contraseña { get; set; }
        public string Rol { get; set; }
    }
}
