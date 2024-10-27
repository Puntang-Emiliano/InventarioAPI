using InventarioAPI.Data;
using InventarioAPI.DTOs;
using InventarioAPI.Models;
using InventarioAPI.ModelsDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly InventarioContex _context;

        public PedidoController(InventarioContex context)
        {
            _context = context;
        }

        // Obtener todos los pedidos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PedidoDTO>>> GetPedidos()
        {
            var pedidos = await _context.Pedidos
                .Include(p => p.DetallePedido)
                .Select(p => new PedidoDTO
                {
                    IdPedido = p.IdPedido,
                    FechaPedido = p.FechaPedido,
                    Total = p.Total,
                    ClienteId = p.ClienteId,
                    DetallesPedido = p.DetallePedido.Select(dp => new DetallePedidoDTO
                    {
                        IdDetallePedido = dp.IdDetallePedido,
                        Cantidad = dp.Cantidad,
                        PrecioUnitario = dp.PrecioUnitario,
                        ProductoId = dp.ProductoId
                    }).ToList()
                }).ToListAsync();

            return Ok(pedidos);
        }

        // Obtener un pedido por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<PedidoDTO>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.DetallePedido)
                .Where(p => p.IdPedido == id)
                .Select(p => new PedidoDTO
                {
                    IdPedido = p.IdPedido,
                    FechaPedido = p.FechaPedido,
                    Total = p.Total,
                    ClienteId = p.ClienteId,
                    DetallesPedido = p.DetallePedido.Select(dp => new DetallePedidoDTO
                    {
                        IdDetallePedido = dp.IdDetallePedido,
                        Cantidad = dp.Cantidad,
                        PrecioUnitario = dp.PrecioUnitario,
                        ProductoId = dp.ProductoId
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (pedido == null)
            {
                return NotFound();
            }

            return Ok(pedido);
        }

        // Crear un nuevo pedido
        [HttpPost]
        public async Task<ActionResult<PedidoDTO>> PostPedido(CrearPedidoDTO crearPedidoDto)
        {
            var pedido = new Pedido
            {
                FechaPedido = DateTime.Now,
                Total = crearPedidoDto.Total,
                ClienteId = crearPedidoDto.ClienteId,
                DetallePedido = crearPedidoDto.DetallesPedido.Select(dp => new DetallePedido
                {
                    Cantidad = dp.Cantidad,
                    PrecioUnitario = dp.PrecioUnitario,
                    ProductoId = dp.ProductoId
                }).ToList()
            };

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.IdPedido }, pedido);
        }

        // Actualizar un pedido
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, ModificarPedidoDTO modificarPedidoDto)
        {
            var pedidoExistente = await _context.Pedidos
                .Include(p => p.DetallePedido) 
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedidoExistente == null)
            {
                return NotFound();
            }

            pedidoExistente.Total = modificarPedidoDto.Total;
            pedidoExistente.ClienteId = modificarPedidoDto.ClienteId;

            foreach (var detalleDto in modificarPedidoDto.DetallesPedido)
            {
                var detalleExistente = pedidoExistente.DetallePedido
                    .FirstOrDefault(dp => dp.IdDetallePedido == detalleDto.IdDetallePedido);

                if (detalleExistente != null)
                {
             
                    detalleExistente.Cantidad = detalleDto.Cantidad;
                    detalleExistente.PrecioUnitario = detalleDto.PrecioUnitario;
                    detalleExistente.ProductoId = detalleDto.ProductoId;
                }
                else
                {

                    var nuevoDetalle = new DetallePedido
                    {
                        Cantidad = detalleDto.Cantidad,
                        PrecioUnitario = detalleDto.PrecioUnitario,
                        ProductoId = detalleDto.ProductoId
                    };
                    pedidoExistente.DetallePedido.Add(nuevoDetalle);
                }
            }

            // Guardar los cambios en la base de datos
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // Eliminar un pedido
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos.Include(p => p.DetallePedido).FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound();
            }

            // Eliminar los detalles de pedido relacionados
            if (pedido.DetallePedido != null && pedido.DetallePedido.Any())
            {
                _context.DetallePedido.RemoveRange(pedido.DetallePedido);
            }

            // Eliminar el pedido
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

    }
}
