using InventarioAPI.Data;
using InventarioAPI.Models;
using InventarioAPI.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
  
    public class OrdenCompraController : ControllerBase
    {
        private readonly InventarioContex _context;

        public OrdenCompraController(InventarioContex context)
        {
            _context = context;
        }

        // muestra las órdenes de compra
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrdenCompraDTO>>> GetOrdenCompra()
        {
            var ordenes = await _context.OrdenCompra
                .Include(o => o.Proveedor)
                .Include(o => o.DetalleOrdenCompra)
                .Select(o => new OrdenCompraDTO
                {
                    IdOrdenCompra = o.IdOrdenCompra,
                    FechaOrden = o.FechaOrden,
                    Estado = o.Estado,
                    Total = o.Total,
                    ProveedorId = o.ProveedorId,
                    NombreProveedor = o.Proveedor.Nombre,
                    DetalleOrdenCompra = o.DetalleOrdenCompra.Select(d => new DetalleOrdenCompraDTO
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    }).ToList()
                }).ToListAsync();

            return Ok(ordenes);
        }

        // muestra una orden de compra por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<OrdenCompraDTO>> GetOrdenCompra(int id)
        {
            var orden = await _context.OrdenCompra
                .Include(o => o.Proveedor)
                .Include(o => o.DetalleOrdenCompra)
                .Where(o => o.IdOrdenCompra == id)
                .Select(o => new OrdenCompraDTO
                {
                    IdOrdenCompra = o.IdOrdenCompra,
                    FechaOrden = o.FechaOrden,
                    Estado = o.Estado,
                    Total = o.Total,
                    ProveedorId = o.ProveedorId,
                    NombreProveedor = o.Proveedor.Nombre,
                    DetalleOrdenCompra = o.DetalleOrdenCompra.Select(d => new DetalleOrdenCompraDTO
                    {
                        ProductoId = d.ProductoId,
                        Cantidad = d.Cantidad,
                        PrecioUnitario = d.PrecioUnitario
                    }).ToList()
                }).FirstOrDefaultAsync();

            if (orden == null)
            {
                return NotFound();
            }

            return Ok(orden);
        }

        // Crear una orden
        [HttpPost]
        public async Task<ActionResult<OrdenCompraDTO>> PostOrdenCompra(CrearOrdenCompraDTO crearOrdenDto)
        {
            if (!ValidarEstado(crearOrdenDto.Estado))
            {
                return BadRequest("El estado debe ser 'Pendiente', 'Completa' o 'Cancelada'.");
            }

            var orden = new OrdenCompra
            {
                FechaOrden = DateTime.Now,
                Estado = crearOrdenDto.Estado,
                Total = crearOrdenDto.Total,
                ProveedorId = crearOrdenDto.ProveedorId,
                DetalleOrdenCompra = crearOrdenDto.DetalleOrdenCompra.Select(d => new DetalleOrdenCompra
                {
                    ProductoId = d.ProductoId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario
                }).ToList()
            };

            _context.OrdenCompra.Add(orden);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrdenCompra), new { id = orden.IdOrdenCompra }, orden);
        }

        // modifica una orden 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenCompra(int id, ModificarOrdenCompraDTO crearOrdenDto)
        {
            var ordenExistente = await _context.OrdenCompra.FindAsync(id);

            if (ordenExistente == null)
            {
                return NotFound();
            }

            if (!ValidarEstado(crearOrdenDto.Estado))
            {
                return BadRequest("El estado debe ser 'Pendiente', 'Completa' o 'Cancelada'.");
            }

            ordenExistente.Estado = crearOrdenDto.Estado;
            ordenExistente.Total = crearOrdenDto.Total;
            ordenExistente.ProveedorId = crearOrdenDto.ProveedorId;
            ordenExistente.DetalleOrdenCompra = crearOrdenDto.DetalleOrdenCompra.Select(d => new DetalleOrdenCompra
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            }).ToList();

            _context.Entry(ordenExistente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Elimina una orden
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenCompra(int id)
        {
            var orden = await _context.OrdenCompra.FindAsync(id);

            if (orden == null)
            {
                return NotFound();
            }

            _context.OrdenCompra.Remove(orden);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Validar el estado de la orden
        private bool ValidarEstado(string estado)
        {
            return estado == "Pendiente" || estado == "Completa" || estado == "Cancelada";
        }
    }
}
