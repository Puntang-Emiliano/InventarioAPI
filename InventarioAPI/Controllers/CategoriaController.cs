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

    public class CategoriaController : Controller
    {
        
        private readonly InventarioContex _context;

        public CategoriaController(InventarioContex context)
        {
            _context = context;
        }

        // trae todas las Categorias
        [Authorize]
      [HttpGet(Name = "ObtenerTodosCat")]
        public async Task<IActionResult> ObtenerTodos()
        {
            try
            {
                var lista = await _context.Categorias.ToListAsync();
                return Ok(lista);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        // trae  Categorias por Id
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Categoria>> ObtenerCategoriaPorId(int id)
        {
            var categoria = await _context.Categorias.FindAsync(id);
            if (categoria == null)
            {
                return NotFound();
            }
            return Ok(categoria);
        }

        // ingresa una nueva la categoria
        [HttpPost]
        public async Task<ActionResult<Categoria>> CrearCategoria([FromBody] CrearCategoriaDTo categoriaDto)
        {
            var nuevaCategoria = new Categoria
            {
                Nombre = categoriaDto.Nombre,
                Descripcion = categoriaDto.Descripcion
            };

            _context.Categorias.Add(nuevaCategoria);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(ObtenerCategoriaPorId), new { id = nuevaCategoria.IdCategoria }, nuevaCategoria);
        }

       // Modificar una Categoria
        [HttpPut("{id:int}")]
        public async Task<IActionResult> ModificarCategoria([FromBody] ModificarCategoriaDTO categoriaDto, [FromRoute] int id)
        {
            var categoriaExistente = await _context.Categorias.FindAsync(id);
            if (categoriaExistente == null)
            {
                return NotFound();
            }

            if (!string.IsNullOrEmpty(categoriaDto.Nombre))
                categoriaExistente.Nombre = categoriaDto.Nombre;

            if (!string.IsNullOrEmpty(categoriaDto.Descripcion))
                categoriaExistente.Descripcion = categoriaDto.Descripcion;

            _context.Categorias.Update(categoriaExistente);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Eliminar una Categoria por Id
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> EliminarCategoria(int id)
        {
            var categoriaExistente = await _context.Categorias.FindAsync(id);
            if (categoriaExistente == null)
            {
                return NotFound();
            }

            _context.Categorias.Remove(categoriaExistente);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
