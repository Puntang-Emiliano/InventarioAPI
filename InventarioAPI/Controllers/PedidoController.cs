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
                    IdUsuario = p.IdUsuario,
                    Estado = p.Estado,
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
                    IdUsuario = p.IdUsuario,  
                    Estado = p.Estado,
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

        //[HttpPost]
        //public async Task<ActionResult<PedidoDTO>> PostPedido(CrearPedidoDTO crearPedidoDto)
        //{
        //    var pedido = new Pedido
        //    {
        //        FechaPedido = DateTime.Now,
        //        Total = crearPedidoDto.Total,
        //        IdUsuario = crearPedidoDto.IdUsuario,  
        //        Estado = "Pendiente", // Estado inicial
        //        DetallePedido = crearPedidoDto.DetallesPedido.Select(dp => new DetallePedido
        //        {
        //            Cantidad = dp.Cantidad,
        //            PrecioUnitario = dp.PrecioUnitario,
        //            ProductoId = dp.ProductoId
        //        }).ToList()
        //    };

        //    _context.Pedidos.Add(pedido);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction(nameof(GetPedido), new { id = pedido.IdPedido }, pedido);
        //}

        // modificar solo el estado



        [HttpPost]
        public async Task<ActionResult<PedidoDTO>> PostPedido(CrearPedidoDTO crearPedidoDto)
        {
            var productos = await _context.Productos
                                          .Where(p => crearPedidoDto.DetallesPedido.Select(dp => dp.ProductoId).Contains(p.IdProducto))
                                          .ToListAsync();
            var pedido = new Pedido
            {
                FechaPedido = DateTime.Now,
                Total = 0, 
                IdUsuario = crearPedidoDto.IdUsuario,
                Estado = "Pendiente", // Estado inicial
                DetallePedido = new List<DetallePedido>()
            };
            foreach (var dp in crearPedidoDto.DetallesPedido)
            {
                var producto = productos.FirstOrDefault(p => p.IdProducto == dp.ProductoId);

                if (producto == null)
                {
                    return BadRequest($"Producto con ID {dp.ProductoId} no encontrado.");
                }

                var detalle = new DetallePedido
                {
                    Cantidad = dp.Cantidad,
                    PrecioUnitario = producto.Precio, 
                    ProductoId = dp.ProductoId
                };
                pedido.DetallePedido.Add(detalle);

                pedido.Total += detalle.Cantidad * detalle.PrecioUnitario;
            }

            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPedido), new { id = pedido.IdPedido }, pedido);
        }




        [HttpPut("{id}")]
        public async Task<IActionResult> PutPedido(int id, ModificarEstadoPedidoDTO modificarEstadoDto)
        {
            var pedidoExistente = await _context.Pedidos
                .Include(p => p.DetallePedido)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedidoExistente == null)
            {
                return NotFound();
            }
            var estadoAnterior = pedidoExistente.Estado;
            var nuevoEstado = modificarEstadoDto.Estado;

            if (estadoAnterior == "Procesado" && nuevoEstado != "Procesado")
            {
                foreach (var detalle in pedidoExistente.DetallePedido)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad;
                    }
                }
            }
            else if (estadoAnterior != "Procesado" && nuevoEstado == "Procesado")
            {
                foreach (var detalle in pedidoExistente.DetallePedido)
                {
                    var producto = await _context.Productos.FindAsync(detalle.ProductoId);
                    if (producto != null)
                    {
                        if (producto.Stock < detalle.Cantidad)
                        {
                            return BadRequest($"Stock insuficiente para el producto ID {detalle.ProductoId}");
                        }
                        producto.Stock -= detalle.Cantidad;
                    }
                }
            }

            pedidoExistente.Estado = nuevoEstado;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePedido(int id)
        {
            var pedido = await _context.Pedidos.Include(p => p.DetallePedido).FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound();
            }
            if (pedido.DetallePedido != null && pedido.DetallePedido.Any())
            {
                _context.DetallePedido.RemoveRange(pedido.DetallePedido);
            }
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // metodo para aceptar pedido y descontar stock
        [HttpPut("{id}/aceptar")]
        public async Task<IActionResult> AceptarPedido(int id)
        {
            var pedido = await _context.Pedidos
                .Include(p => p.DetallePedido)
                .ThenInclude(dp => dp.Producto)
                .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound("El pedido no existe.");
            }
            if (pedido.Estado == "Procesado" && (pedido.Estado == "Pendiente" || pedido.Estado == "Rechazado"))
            {
                foreach (var detalle in pedido.DetallePedido)
                {
                    var producto = await _context.Productos.FirstOrDefaultAsync(p => p.IdProducto == detalle.ProductoId);
                    if (producto == null)
                    {
                        return NotFound($"Producto con ID {detalle.ProductoId} no encontrado.");
                    }
                    producto.Stock += detalle.Cantidad;
                }
            }
            else if (pedido.Estado == "Pendiente")
            {
                foreach (var detalle in pedido.DetallePedido)
                {
                    var producto = await _context.Productos.FirstOrDefaultAsync(p => p.IdProducto == detalle.ProductoId);
                    if (producto == null)
                    {
                        return NotFound($"Producto con ID {detalle.ProductoId} no encontrado.");
                    }

                    if (producto.Stock < detalle.Cantidad)
                    {
                        return BadRequest($"Stock insuficiente para el producto: {producto.Nombre}");
                    }
                    producto.Stock -= detalle.Cantidad;
                }

                pedido.Estado = "Procesado";
            }
            await _context.SaveChangesAsync();

            return Ok("Pedido Procesado y stock actualizado.");
        }

        [HttpPut("{id}/rechazar")]
        public async Task<IActionResult> RechazarPedido(int id)
        {
            var pedido = await _context.Pedidos
                                        .Include(p => p.DetallePedido)
                                        .FirstOrDefaultAsync(p => p.IdPedido == id);

            if (pedido == null)
            {
                return NotFound("El pedido no existe.");
            }
            if (pedido.Estado == "Pendiente" || pedido.Estado == "Rechazado")
            {
                pedido.Estado = "Rechazado";
            }
            else if (pedido.Estado == "Procesado")
            {
                foreach (var detalle in pedido.DetallePedido)
                {
                    var producto = await _context.Productos
                                                  .FirstOrDefaultAsync(p => p.IdProducto == detalle.ProductoId);

                    if (producto != null)
                    {
                        producto.Stock += detalle.Cantidad;
                    }
                }

                pedido.Estado = "Rechazado";
            }
            else
            {
                return BadRequest("El estado del pedido no es válido para esta acción.");
            }

            await _context.SaveChangesAsync();

            return Ok("Pedido rechazado y stock actualizado.");
        }


        [HttpGet("usuario/{idUsuario}")]
        public async Task<ActionResult<IEnumerable<PedidoDTO>>> GetPedidosPorUsuario(int idUsuario)
        {
            var pedidos = await _context.Pedidos
                .Where(p => p.IdUsuario == idUsuario) // Filtrar por el IdUsuario (cliente)
                .Include(p => p.DetallePedido)
                .Select(p => new PedidoDTO
                {
                    IdPedido = p.IdPedido,
                    FechaPedido = p.FechaPedido,
                    Total = p.Total,
                    IdUsuario = p.IdUsuario,
                    Estado = p.Estado,
                    DetallesPedido = p.DetallePedido.Select(dp => new DetallePedidoDTO
                    {
                        IdDetallePedido = dp.IdDetallePedido,
                        Cantidad = dp.Cantidad,
                        PrecioUnitario = dp.PrecioUnitario,
                        ProductoId = dp.ProductoId
                    }).ToList()
                }).ToListAsync();

            if (pedidos == null || !pedidos.Any())
            {
                return NotFound("No se encontraron pedidos para el usuario.");
            }

            return Ok(pedidos);
        }
    }
}
