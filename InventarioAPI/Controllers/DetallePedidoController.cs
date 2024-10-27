using InventarioAPI.Data;
using InventarioAPI.Models;
using InventarioAPI.ModelsDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DetallePedidoController : ControllerBase
    {
        private readonly InventarioContex _context;

        public DetallePedidoController(InventarioContex context)
        {
            _context = context;
        }

        // Obtener todos los detalles de pedido
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetallePedidoDTO>>> GetDetallesPedido()
        {
            var detalles = await _context.DetallePedido
                .Include(dp => dp.Producto) // Incluir la información del producto
                .Select(dp => new DetallePedidoDTO
                {
                    IdDetallePedido = dp.IdDetallePedido,
                    Cantidad = dp.Cantidad,
                    PrecioUnitario = dp.PrecioUnitario,
                    ProductoId = dp.ProductoId
                }).ToListAsync();

            return Ok(detalles);
        }

        // Obtener un detalle de pedido por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<DetallePedidoDTO>> GetDetallePedido(int id)
        {
            var detalle = await _context.DetallePedido
                .Include(dp => dp.Producto) // Incluir la información del producto
                .Where(dp => dp.IdDetallePedido == id)
                .Select(dp => new DetallePedidoDTO
                {
                    IdDetallePedido = dp.IdDetallePedido,
                    Cantidad = dp.Cantidad,
                    PrecioUnitario = dp.PrecioUnitario,
                    ProductoId = dp.ProductoId
                }).FirstOrDefaultAsync();

            if (detalle == null)
            {
                return NotFound();
            }

            return Ok(detalle);
        }
    }
}
