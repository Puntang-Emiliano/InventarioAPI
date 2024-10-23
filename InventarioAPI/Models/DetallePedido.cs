using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models
{
    public class DetallePedido
    {
        [Key]
        public int IdDetallePedido { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }

        // Relación con Pedido
        public int PedidoId { get; set; }
        public Pedido Pedido { get; set; }

        // Relación con Producto
        public int ProductoId { get; set; }
        public Producto Producto { get; set; }
    }

}
