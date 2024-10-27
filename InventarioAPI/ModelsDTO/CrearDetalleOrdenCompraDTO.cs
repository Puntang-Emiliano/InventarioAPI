namespace InventarioAPI.ModelsDTO
{
    public class CrearDetalleOrdenCompraDTO
    {
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public int OrdenCompraId { get; set; }
        public int ProductoId { get; set; }
    }
}
