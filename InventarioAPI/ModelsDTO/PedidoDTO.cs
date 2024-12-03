namespace InventarioAPI.ModelsDTO
{
    public class PedidoDTO
    {
        public int IdPedido { get; set; }
        public DateTime FechaPedido { get; set; }
        public decimal Total { get; set; }
        public int ClienteId { get; set; }
        public string Estado { get; set; }
        public List<DetallePedidoDTO> DetallesPedido { get; set; }
    }
}
