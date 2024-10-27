namespace InventarioAPI.ModelsDTO
{
    public class OrdenCompraDTO
    {
        public int IdOrdenCompra { get; set; }
        public DateTime FechaOrden { get; set; }
        public string Estado { get; set; }
        public decimal Total { get; set; }
        public int ProveedorId { get; set; }
        public string NombreProveedor { get; set; }
        public ICollection<DetalleOrdenCompra1DTO> DetalleOrdenCompra { get; set; }
    }
}
