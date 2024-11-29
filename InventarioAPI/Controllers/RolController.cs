using InventarioAPI.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RolController : Controller
    {

        private readonly InventarioContex _context;

        public RolController(InventarioContex context)
        {
            _context = context;
        }

        [HttpGet(Name = "GetRol")]
        public async Task<IActionResult> GetRol()
        {
            try
            {
                var lista = await _context.Roles.ToListAsync();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
