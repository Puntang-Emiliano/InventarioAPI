using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.Now;
        public decimal Total { get; set; }

        // Relación con Cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        // Relación con DetallePedido
        public ICollection<DetallePedido> DetallePedido { get; set; }
    }

}
