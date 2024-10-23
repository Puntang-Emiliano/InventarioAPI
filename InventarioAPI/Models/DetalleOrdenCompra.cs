using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models
{
    public class DetalleOrdenCompra
    {
        [Key]
        public int IdDetalleOrdenCompra { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Relación con OrdenCompra
        public int OrdenCompraId { get; set; }
        public OrdenCompra OrdenCompra { get; set; }

        // Relación con Producto
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
    }

}
