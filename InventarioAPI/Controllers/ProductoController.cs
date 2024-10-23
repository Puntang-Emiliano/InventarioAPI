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
    [Authorize]
    public class ProductoController : ControllerBase
    {
        private readonly InventarioContex _context;

        public ProductoController(InventarioContex context)
        { 
            _context = context;
        }

        // Traemos la lista de todos los productos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductos()
        {
            var productos = await _context.Productos
                .Select(p => new ProductoDTO
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    CategoriaId = p.CategoriaId,
                    ProveedorId = p.ProveedorId
                }).ToListAsync();

            return Ok(productos);
        }
        // Montramos un producto pasando un Id
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductoDTO>> GetProducto(int id)
        {
            var producto = await _context.Productos
                .Where(p => p.IdProducto == id)
                .Select(p => new ProductoDTO
                {
                    IdProducto = p.IdProducto,
                    Nombre = p.Nombre,
                    Descripcion = p.Descripcion,
                    Precio = p.Precio,
                    Stock = p.Stock,
                    CategoriaId = p.CategoriaId,
                    ProveedorId = p.ProveedorId
                }).FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

       // Insertamos un nuevo Producto
        [HttpPost]
        public async Task<ActionResult<ProductoDTO>> PostProducto(CrearProductoDTO crearProductoDto)
        {
            var producto = new Producto
            {
                Nombre = crearProductoDto.Nombre,
                Descripcion = crearProductoDto.Descripcion,
                Precio = crearProductoDto.Precio,
                Stock = crearProductoDto.Stock,
                CategoriaId = crearProductoDto.CategoriaId,
                ProveedorId = crearProductoDto.ProveedorId
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();

            var productoDto = new ProductoDTO
            {
                IdProducto = producto.IdProducto,
                Nombre = producto.Nombre,
                Descripcion = producto.Descripcion,
                Precio = producto.Precio,
                Stock = producto.Stock,
                CategoriaId = producto.CategoriaId,
                ProveedorId = producto.ProveedorId
            };

            return CreatedAtAction(nameof(GetProducto), new { id = productoDto.IdProducto }, productoDto);
        }

        //Eliminamos un producto pasandole el Id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ProductoExists(int id)
        {
            return _context.Productos.Any(e => e.IdProducto == id);
        }


        [HttpPut("{productoId:int}")]
        public async Task<IActionResult> Modificar([FromBody] ModificarProductoDTO productoDto, [FromRoute] int productoId)
        {
            try
            {
                var productoExistente = await _context.Productos.FindAsync(productoId);

                if (productoExistente != null)
                {
                    
                    if (!string.IsNullOrEmpty(productoDto.Nombre))
                        productoExistente.Nombre = productoDto.Nombre;

                    if (!string.IsNullOrEmpty(productoDto.Descripcion))
                        productoExistente.Descripcion = productoDto.Descripcion;

                    if (productoDto.Precio >= 0) // valida que el precio no sea negativo
                        productoExistente.Precio = productoDto.Precio;

                    if (productoDto.Stock >= 0) // valida que el stock puede ser 0 o más
                        productoExistente.Stock = productoDto.Stock;               

                    _context.Productos.Update(productoExistente);
                    await _context.SaveChangesAsync();

                    return NoContent();
                }

                return NotFound();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
