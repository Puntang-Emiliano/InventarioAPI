using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models
{
    public class Proveedor
    {
        [Key]
        public int IdProveedor { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Email { get; set; }

      
        public ICollection<Producto> Productos { get; set; }
    }

}
