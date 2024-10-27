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

    public class ClienteController : ControllerBase
    {
        private readonly InventarioContex _context;

        public ClienteController(InventarioContex context)
        {
            _context = context;
        }

        // muestra todos los clientes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ClienteDTO>>> GetClientes()
        {
            var clientes = await _context.Clientes
                .Select(c => new ClienteDTO
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    Email = c.Email,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion
                }).ToListAsync();

            return Ok(clientes);
        }

        // muestra cliente por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<ClienteDTO>> GetCliente(int id)
        {
            var cliente = await _context.Clientes
                .Where(c => c.IdCliente == id)
                .Select(c => new ClienteDTO
                {
                    IdCliente = c.IdCliente,
                    Nombre = c.Nombre,
                    Email = c.Email,
                    Telefono = c.Telefono,
                    Direccion = c.Direccion
                }).FirstOrDefaultAsync();

            if (cliente == null)
            {
                return NotFound();
            }

            return Ok(cliente);
        }

        // Crear  cliente
        [HttpPost]
        public async Task<ActionResult<ClienteDTO>> PostCliente(CrearClienteDTO crearClienteDto)
        {
            var cliente = new Cliente
            {
                Nombre = crearClienteDto.Nombre,
                Email = crearClienteDto.Email,
                Telefono = crearClienteDto.Telefono,
                Direccion = crearClienteDto.Direccion
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            var clienteDto = new ClienteDTO
            {
                IdCliente = cliente.IdCliente,
                Nombre = cliente.Nombre,
                Email = cliente.Email,
                Telefono = cliente.Telefono,
                Direccion = cliente.Direccion
            };

            return CreatedAtAction(nameof(GetCliente), new { id = clienteDto.IdCliente }, clienteDto);
        }

        // modifica un cliente 
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCliente(int id, CrearClienteDTO crearClienteDto)
        {
            var clienteExistente = await _context.Clientes.FindAsync(id);

            if (clienteExistente == null)
            {
                return NotFound();
            }

            clienteExistente.Nombre = crearClienteDto.Nombre;
            clienteExistente.Email = crearClienteDto.Email;
            clienteExistente.Telefono = crearClienteDto.Telefono;
            clienteExistente.Direccion = crearClienteDto.Direccion;

            _context.Entry(clienteExistente).State = EntityState.Modified;
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Eliminar un cliente 
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCliente(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                return NotFound();
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
