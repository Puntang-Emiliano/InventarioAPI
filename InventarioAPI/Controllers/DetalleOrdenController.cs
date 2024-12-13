using InventarioAPI.Data;
using InventarioAPI.Models;
using InventarioAPI.ModelsDTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DetalleOrdenCompraController : ControllerBase
    {
        private readonly InventarioContex _context;

        public DetalleOrdenCompraController(InventarioContex context)
        {
            _context = context;
        }

        // Obtener todos los detalles de orden de compra
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetalleOrdenCompraDTO>>> GetDetallesOrdenCompra()
        {
            var detalles = await _context.DetalleOrdenCompra
                .Include(d => d.Producto)
                .Select(d => new DetalleOrdenCompraDTO
                {
                    IdDetalleOrdenCompra = d.IdDetalleOrdenCompra,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    OrdenCompraId = d.OrdenCompraId,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre
                }).ToListAsync();

            return Ok(detalles);
        }

        // Obtener un detalle de orden de compra por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<DetalleOrdenCompraDTO>> GetDetalleOrdenCompra(int id)
        {
            var detalle = await _context.DetalleOrdenCompra
                .Include(d => d.Producto)
                .Where(d => d.IdDetalleOrdenCompra == id)
                .Select(d => new DetalleOrdenCompraDTO
                {
                    IdDetalleOrdenCompra = d.IdDetalleOrdenCompra,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    OrdenCompraId = d.OrdenCompraId,
                    ProductoId = d.ProductoId,
                    NombreProducto = d.Producto.Nombre
                }).FirstOrDefaultAsync();

            if (detalle == null)
            {
                return NotFound();
            }

            return Ok(detalle);
        }

    //    // Crear un nuevo detalle de orden de compra
    //    [HttpPost]
    //    public async Task<ActionResult<DetalleOrdenCompraDTO>> PostDetalleOrdenCompra(CrearDetalleOrdenCompraDTO crearDetalleDto)
    //    {
    //        var detalle = new DetalleOrdenCompra
    //        {
    //            Cantidad = crearDetalleDto.Cantidad,
    //            PrecioUnitario = crearDetalleDto.PrecioUnitario,
    //            OrdenCompraId = crearDetalleDto.OrdenCompraId,
    //            ProductoId = crearDetalleDto.ProductoId
    //        };

    //        _context.DetalleOrdenCompra.Add(detalle);
    //        await _context.SaveChangesAsync();

    //        return CreatedAtAction(nameof(GetDetalleOrdenCompra), new { id = detalle.IdDetalleOrdenCompra }, detalle);
    //    }

    //    // Actualizar un detalle de orden de compra
    //    [HttpPut("{id}")]
    //    public async Task<IActionResult> PutDetalleOrdenCompra(int id, CrearDetalleOrdenCompraDTO crearDetalleDto)
    //    {
    //        var detalleExistente = await _context.DetalleOrdenCompra.FindAsync(id);

    //        if (detalleExistente == null)
    //        {
    //            return NotFound();
    //        }

    //        detalleExistente.Cantidad = crearDetalleDto.Cantidad;
    //        detalleExistente.PrecioUnitario = crearDetalleDto.PrecioUnitario;
    //        detalleExistente.OrdenCompraId = crearDetalleDto.OrdenCompraId;
    //        detalleExistente.ProductoId = crearDetalleDto.ProductoId;

    //        _context.Entry(detalleExistente).State = EntityState.Modified;
    //        await _context.SaveChangesAsync();

    //        return NoContent();
    //    }

    //    // Eliminar un detalle de orden de compra
    //    [HttpDelete("{id}")]
    //    public async Task<IActionResult> DeleteDetalleOrdenCompra(int id)
    //    {
    //        var detalle = await _context.DetalleOrdenCompra.FindAsync(id);

    //        if (detalle == null)
    //        {
    //            return NotFound();
    //        }

    //        _context.DetalleOrdenCompra.Remove(detalle);
    //        await _context.SaveChangesAsync();

    //        return NoContent();
    //    }
    }
}
