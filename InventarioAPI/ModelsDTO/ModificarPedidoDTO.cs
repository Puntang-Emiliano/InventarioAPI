using InventarioAPI.ModelsDTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.DTOs
{
    public class ModificarPedidoDTO
    {

        public decimal Total { get; set; }
        public int ClienteId { get; set; }
        public string Estado { get; set; } // Pendiente, Procesado, Rechazado
        public List<DetallePedidoDTO> DetallesPedido { get; set; }
    }

   
}
