using InventarioAPI.Models;

namespace InventarioAPI.ModelsDTO
{
    public class CrearPedidoDTO
    {
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public int IdUsuario { get; set; }
    
        public List<CrearDetallePedidoDTO> DetallesPedido { get; set; }
    }
}
