using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InventarioAPI.Models
{
    public class UsuarioLogingDTO
    {
        [Key]
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Email { get; set; }
        public string Contraseña { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        // Relación con Rol
        public int IdRol { get; set; }  
        [ForeignKey("IdRol")]
        public Rol Rol { get; set; }   
    }
}
