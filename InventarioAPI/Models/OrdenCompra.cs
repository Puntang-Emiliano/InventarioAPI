using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.Models
{
    public class OrdenCompra
    {
        [Key]
        public int IdOrdenCompra { get; set; }
        public DateTime FechaOrden { get; set; } = DateTime.Now;
        public string Estado { get; set; }  // tiene que ser si o si "Pendiente", "Completa", "Cancelada"
        public decimal Total { get; set; }

        // Relación con Proveedor
        public int ProveedorId { get; set; }
        public Proveedor Proveedor { get; set; }

        // Relación con DetallesOrdenCompra
        public ICollection<DetalleOrdenCompra> DetallesOrdenCompra { get; set; }
    }

}
