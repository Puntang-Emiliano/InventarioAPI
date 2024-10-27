using System.Text.Json.Serialization;

namespace InventarioAPI.ModelsDTO
{
    public class CrearOrdenCompraDTO
    {
        public string Estado { get; set; }  //solo permite "Pendiente", "Completa", "Cancelada"
        public decimal Total { get; set; }
        public int ProveedorId { get; set; }

       
        public ICollection<DetalleOrdenCompra1DTO> DetalleOrdenCompra { get; set; }
    }
}
