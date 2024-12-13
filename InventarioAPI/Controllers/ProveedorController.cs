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

    public class ProveedorController : ControllerBase
    {
        private readonly InventarioContex _context;

        public ProveedorController(InventarioContex context)
        {
            _context = context;
        }

        //  todos los proveedores
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProveedorDTO>>> GetProveedores()
        {
            var proveedores = await _context.Proveedores
                .Select(p => new ProveedorDTO
                {
                    IdProveedor = p.IdProveedor,
                    Nombre = p.Nombre,
                    Direccion = p.Direccion,
                    Telefono = p.Telefono,
                    Email = p.Email
                }).ToListAsync();

            return Ok(proveedores);
        }

        // proveedor por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ProveedorDTO>> GetProveedor(int id)
        {
            var proveedor = await _context.Proveedores
                .Where(p => p.IdProveedor == id)
                .Select(p => new ProveedorDTO
                {
                    IdProveedor = p.IdProveedor,
                    Nombre = p.Nombre,
                    Direccion = p.Direccion,
                    Telefono = p.Telefono,
                    Email = p.Email
                }).FirstOrDefaultAsync();

            if (proveedor == null)
            {
                return NotFound();
            }

            return Ok(proveedor);
        }

        // Crear proveedor
        [HttpPost]
        public async Task<ActionResult<ProveedorDTO>> PostProveedor(CrearProveedorDTO crearProveedorDto)
        {
            var proveedor = new Proveedor
            {
                Nombre = crearProveedorDto.Nombre,
                Direccion = crearProveedorDto.Direccion,
                Telefono = crearProveedorDto.Telefono,
                Email = crearProveedorDto.Email
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();

            var proveedorDto = new ProveedorDTO
            {
                IdProveedor = proveedor.IdProveedor,
                Nombre = proveedor.Nombre,
                Direccion = proveedor.Direccion,
                Telefono = proveedor.Telefono,
                Email = proveedor.Email
            };

            return CreatedAtAction(nameof(GetProveedor), new { id = proveedorDto.IdProveedor }, proveedorDto);
        }

        // Modifica proveedor 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProveedor(int id, CrearProveedorDTO crearProveedorDto)
        {
            var proveedorExistente = await _context.Proveedores.FindAsync(id);

            if (proveedorExistente == null)
            {
                return NotFound();
            }

            proveedorExistente.Nombre = crearProveedorDto.Nombre;
            proveedorExistente.Direccion = crearProveedorDto.Direccion;
            proveedorExistente.Telefono = crearProveedorDto.Telefono;
            proveedorExistente.Email = crearProveedorDto.Email;

            _context.Entry(proveedorExistente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Eliminar un proveedor 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProveedor(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);

            if (proveedor == null)
            {
                return NotFound();
            }

            _context.Proveedores.Remove(proveedor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //Productos por Proveedor
        [HttpGet("PorProducto/{proveedorId}")]
        public async Task<ActionResult<IEnumerable<ProductoDTO>>> GetProductosPorProveedor(int proveedorId)
        {
            var productos = await _context.Productos
                .Where(p => p.ProveedorId == proveedorId)  
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

            if (!productos.Any())
            {
                return NotFound("No se encontraron productos para este proveedor.");
            }

            return Ok(productos);  
        }



    }
}
