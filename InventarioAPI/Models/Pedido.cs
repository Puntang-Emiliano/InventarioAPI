using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models
{
    public class Pedido
    {
        [Key]
        public int IdPedido { get; set; }
        public DateTime FechaPedido { get; set; } = DateTime.Now;
        public string Estado { get; set; }
        public decimal Total { get; set; }
        public int IdUsuario { get; set; }

        public ICollection<DetallePedido> DetallePedido { get; set; }
    }

}
