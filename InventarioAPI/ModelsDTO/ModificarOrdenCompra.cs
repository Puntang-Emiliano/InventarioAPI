using System.Text.Json.Serialization;

namespace InventarioAPI.ModelsDTO
{
    public class ModificarOrdenCompraDTO
    {
        public string Estado { get; set; }  //solo permite "Pendiente", "Completa", "Cancelada"
        public decimal Total { get; set; }
        public int ProveedorId { get; set; }

        public ICollection<DetalleOrdenCompraDTO> DetalleOrdenCompra { get; set; }
    }
}
