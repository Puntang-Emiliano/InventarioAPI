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

        [HttpGet("datos-iniciales")]
        public async Task<IActionResult> GetDatosIniciales()
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

            var categorias = await _context.Categorias
                .Select(c => new CategoriaDTO
                {
                    IdCategoria = c.IdCategoria,
                    Nombre = c.Nombre
                }).ToListAsync();

            return Ok(new
            {
                Proveedores = proveedores,
                Productos = productos,
                Categorias = categorias
            });
        }



        // muestra las órdenes de compra
        [HttpGet("obtenerOrdenes")]
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
                    DetalleOrdenCompra = o.DetalleOrdenCompra.Select(d => new DetalleOrdenCompra1DTO
                    {
                        ProductoId = d.ProductoId,
                       // ProductoNombre = d.Producto.Nombre, 
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
                    DetalleOrdenCompra = o.DetalleOrdenCompra.Select(d => new DetalleOrdenCompra1DTO
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


        [HttpPost]
        public async Task<IActionResult> PostOrdenCompra(CrearOrdenCompraDTO crearOrdenDto)
        {
          
            if (!ValidarEstado(crearOrdenDto.Estado))
            {
                return BadRequest("El estado debe ser 'Pendiente', 'Completa' o 'Cancelada'.");
            }

            var productosExistentes = await _context.Productos.ToListAsync();

            // Solo ajustar el stock si la orden está "Procesada"
            if (crearOrdenDto.Estado == "Completa")
            {
                foreach (var detalle in crearOrdenDto.DetalleOrdenCompra)
                {
                    var productoExistente = productosExistentes.FirstOrDefault(p => p.IdProducto == detalle.ProductoId);

                    if (productoExistente != null)
                    {
                        // Ajuste del stock cuando el estado es "Procesada"
                        productoExistente.Stock += detalle.Cantidad;
                        _context.Entry(productoExistente).State = EntityState.Modified;
                    }
                    else
                    {
                        return BadRequest($"El producto con ID {detalle.ProductoId} no existe. Por favor, regístrelo antes de continuar.");
                    }
                }
            }

            var orden = new OrdenCompra
            {
                FechaOrden = DateTime.Now,
                Estado = crearOrdenDto.Estado,
                Total = crearOrdenDto.Total,
                ProveedorId = crearOrdenDto.ProveedorId,
                DetalleOrdenCompra = new List<DetalleOrdenCompra>()
            };

          
            foreach (var detalle in crearOrdenDto.DetalleOrdenCompra)
            {
                var productoExistente = productosExistentes.FirstOrDefault(p => p.IdProducto == detalle.ProductoId);
                if (productoExistente != null)
                {
                    orden.DetalleOrdenCompra.Add(new DetalleOrdenCompra
                    {
                        ProductoId = detalle.ProductoId,
                        Cantidad = detalle.Cantidad,
                        PrecioUnitario = productoExistente.Precio
                    });
                }
                else
                {
                    return BadRequest($"El producto con ID {detalle.ProductoId} no existe.");
                }
            }
            _context.OrdenCompra.Add(orden);
            await _context.SaveChangesAsync();

          
            return CreatedAtAction(nameof(GetOrdenCompra), new { id = orden.IdOrdenCompra }, orden);
        }



        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrdenCompra(int id, ModificarOrdenCompraDTO modificarOrdenDto)
        {
            var ordenExistente = await _context.OrdenCompra
                .Include(o => o.DetalleOrdenCompra) // Incluir los detalles para comparar
                .FirstOrDefaultAsync(o => o.IdOrdenCompra == id);

            if (ordenExistente == null)
            {
                return NotFound();
            }

            if (!ValidarEstado(modificarOrdenDto.Estado))
            {
                return BadRequest("El estado debe ser 'Pendiente', 'Completa' o 'Cancelada'.");
            }

            // Si la orden es Procesada suma el stock si no lo es no. y si cambia de procesada a cancelada lo resta
            if (ordenExistente.Estado != "Completa" && modificarOrdenDto.Estado == "Completa")
            {
                var productosExistentes = await _context.Productos.ToListAsync();

                foreach (var detalle in modificarOrdenDto.DetalleOrdenCompra)
                {
                    var productoExistente = productosExistentes.FirstOrDefault(p => p.IdProducto == detalle.ProductoId);

                    if (productoExistente != null)
                    {
                        productoExistente.Stock += detalle.Cantidad;
                        _context.Entry(productoExistente).State = EntityState.Modified;
                    }
                    else
                    {
                        return BadRequest($"El producto con ID {detalle.ProductoId} no existe.");
                    }
                }
            }
            else if (ordenExistente.Estado == "Completa" && modificarOrdenDto.Estado != "Completa")
            {
              
                var productosExistentes = await _context.Productos.ToListAsync();

                foreach (var detalle in ordenExistente.DetalleOrdenCompra)
                {
                    var productoExistente = productosExistentes.FirstOrDefault(p => p.IdProducto == detalle.ProductoId);

                    if (productoExistente != null)
                    {
                        productoExistente.Stock -= detalle.Cantidad;
                        _context.Entry(productoExistente).State = EntityState.Modified;
                    }
                }
            }
            ordenExistente.Estado = modificarOrdenDto.Estado;
            ordenExistente.Total = modificarOrdenDto.Total;
            ordenExistente.ProveedorId = modificarOrdenDto.ProveedorId;
            ordenExistente.DetalleOrdenCompra = modificarOrdenDto.DetalleOrdenCompra.Select(d => new DetalleOrdenCompra
            {
                ProductoId = d.ProductoId,
                Cantidad = d.Cantidad,
                PrecioUnitario = d.PrecioUnitario
            }).ToList();

            _context.Entry(ordenExistente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }


        // Elimina orden compra
      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrdenCompra(int id)
        {
            var orden = await _context.OrdenCompra.FindAsync(id);

            if (orden == null)
            {
                return NotFound();
            }

            if (orden.Estado != "Cancelada" && orden.Estado != "Pendiente")
            {
                return BadRequest("Solo se pueden eliminar órdenes de compra con estado 'Cancelada' o 'Pendiente'.");
            }

            // Elimina  detalles de la orden de compra antes de eliminar la orden
            var detallesOrden = _context.DetalleOrdenCompra.Where(d => d.OrdenCompraId == id);
            _context.DetalleOrdenCompra.RemoveRange(detallesOrden);

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
