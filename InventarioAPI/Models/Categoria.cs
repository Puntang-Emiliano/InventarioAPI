using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models
{
    public class Categoria
    {
        [Key]
        public int IdCategoria { get; set; }  
        public string Nombre { get; set; }
        public string Descripcion { get; set; }

      
        public ICollection<Producto> Productos { get; set; }
    }

}
