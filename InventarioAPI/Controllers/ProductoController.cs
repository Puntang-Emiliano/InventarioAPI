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
                    ProveedorId = p.ProveedorId,
                    Imagen = string.IsNullOrEmpty(p.Imagen)
                ? null
                : $"{Request.Scheme}://{Request.Host}/Imagenes/{Path.GetFileName(p.Imagen)}"

                })
                .ToListAsync();

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
                    ProveedorId = p.ProveedorId,
                    Imagen = p.Imagen
                }).FirstOrDefaultAsync();

            if (producto == null)
            {
                return NotFound();
            }

            return Ok(producto);
        }

        [HttpPost]
        public async Task<ActionResult<ProductoDTO>> PostProducto([FromBody] CrearProductoDTO crearProductoDto)
        {
            try
            {
               
                var producto = new Producto
                {
                    Nombre = crearProductoDto.Nombre,
                    Descripcion = crearProductoDto.Descripcion,
                    Precio = crearProductoDto.Precio,
                    Stock = crearProductoDto.Stock,
                    CategoriaId = crearProductoDto.CategoriaId,
                    ProveedorId = crearProductoDto.ProveedorId,
                    Imagen = crearProductoDto.Imagen
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
                    ProveedorId = producto.ProveedorId,
                    Imagen = producto.Imagen
                };

                return CreatedAtAction(nameof(GetProducto), new { id = productoDto.IdProducto }, productoDto);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    error = ex.Message,
                    innerException = ex.InnerException?.Message
                });
            }
        }

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
            var productoExistente = await _context.Productos.FindAsync(productoId);

            if (productoExistente == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(productoDto.Nombre))
                productoExistente.Nombre = productoDto.Nombre;

            if (!string.IsNullOrEmpty(productoDto.Descripcion))
                productoExistente.Descripcion = productoDto.Descripcion;

            if (productoDto.Precio >= 0)
                productoExistente.Precio = productoDto.Precio;

            if (productoDto.Stock >= 0)
                productoExistente.Stock = productoDto.Stock;

            if (!string.IsNullOrEmpty(productoDto.Imagen))
                productoExistente.Imagen = productoDto.Imagen;

            _context.Productos.Update(productoExistente);
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // Guardar Imagen

        [HttpPost("GuardarImagen")]
        public async Task<string> GuardarImagen([FromForm] SubirImagen archivo)
        {
            var ruta = String.Empty;

            if (archivo.Imagen.Length > 0)
            {
                var nombreImagen = Guid.NewGuid().ToString() + ".jpg";
                ruta = $"Imagenes/{nombreImagen}";
                using (var stream = new FileStream(ruta, FileMode.Create))
                {
                    await archivo.Imagen.CopyToAsync(stream);

                }
            }
            return ruta;

        }

    }
}
