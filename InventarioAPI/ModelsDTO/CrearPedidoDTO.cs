namespace InventarioAPI.ModelsDTO
{
    public class CrearPedidoDTO
    {
        public decimal Total { get; set; }
        public int ClienteId { get; set; }
        public string Estado { get; set; }
        public List<CrearDetallePedidoDTO> DetallesPedido { get; set; }
    }
}
