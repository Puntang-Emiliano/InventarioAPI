using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models
{
    public class Rol
    {
        [Key]
        public int IdRol { get; set; }
        public string NombreRol { get; set; } 

       
        public ICollection<UsuarioLogingDTO> Usuarios { get; set; }  
    }
}
