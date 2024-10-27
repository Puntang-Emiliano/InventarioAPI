namespace InventarioAPI.ModelsDTO
{
    public class DetallePedidoDTO
    {
        public int IdDetallePedido { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int ProductoId { get; set; }
    }
}
