using InventarioAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PruebaConexion : ControllerBase
    {
        private readonly InventarioContex _context;

        public PruebaConexion(InventarioContex context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult ProbarConexion()
        {
            
            var usuarios = _context.Usuarios.ToList();

            if (usuarios != null)
            {
                return Ok(new { Mensaje = "Conexión exitosa", Usuarios = usuarios });
            }
            else
            {
                return StatusCode(500, "Error al conectarse a la base de datos.");
            }
        }
    }
}
