namespace InventarioAPI.ModelsDTO
{
    public class CrearUsuarioDTO
    {

        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public int IdRol { get; set; }  // 1 para Admin, 2 para Cliente
    }
}
