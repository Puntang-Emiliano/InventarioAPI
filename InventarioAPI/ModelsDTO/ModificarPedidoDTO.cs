using InventarioAPI.Models;
using InventarioAPI.ModelsDTO;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace InventarioAPI.DTOs
{
    public class ModificarPedidoDTO
    {

        public decimal Total { get; set; }
        public string Estado { get; set; } // Pendiente, Procesado, Rechazado
        public int IdUsuario { get; set; }
      
        public List<DetallePedidoDTO> DetallesPedido { get; set; }
    }

   
}
