using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models
{
    public class Producto
    {
        [Key]
        public int IdProducto { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public decimal Precio { get; set; }
        public int Stock { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

       
        public int CategoriaId { get; set; }
        public Categoria Categoria { get; set; }

        // Relación con Proveedor
        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; }

        public string Imagen {  get; set; }
    }

}
