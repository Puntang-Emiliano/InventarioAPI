using InventarioAPI.Data;
using InventarioAPI.Models;
using InventarioAPI.ModelsDTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Security.Cryptography;


namespace InventarioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly InventarioContex _context;
        public UsuarioController(InventarioContex context)
        {
            _context = context;
        }

        // Traemos todos los usuarios
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await _context.Usuarios
                .Include(u => u.Rol)  
                .Select(u => new UsuarioDTO
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    Rol = u.Rol.NombreRol  
                }).ToListAsync();

            return Ok(usuarios);
        }

        // Traemos el usuario por ID
        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDTO>> GetUsuario(int id)
        {
            var usuario = await _context.Usuarios
                .Include(u => u.Rol)  
                .Where(u => u.IdUsuario == id)
                .Select(u => new UsuarioDTO
                {
                    IdUsuario = u.IdUsuario,
                    Nombre = u.Nombre,
                    Email = u.Email,
                    Rol = u.Rol.NombreRol  
                }).FirstOrDefaultAsync();

            if (usuario == null)
            {
                return NotFound();
            }

            return Ok(usuario);
        }

        // Ingresamos un usuario nuevo
        [HttpPost]
        public async Task<ActionResult<UsuarioDTO>> PostUsuario(CrearUsuarioDTO crearUsuarioDto)
        {
            // Encripta la contraseña 
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(crearUsuarioDto.Contraseña));
                var hashedContraseña = Convert.ToBase64String(hashBytes);

                var usuario = new UsuarioLogingDTO
                {
                    Nombre = crearUsuarioDto.Nombre,
                    Email = crearUsuarioDto.Email,
                    Contraseña = hashedContraseña, 
                    IdRol = crearUsuarioDto.IdRol
                };

                _context.Usuarios.Add(usuario);
                await _context.SaveChangesAsync();

                var usuarioDto = new UsuarioDTO
                {
                    IdUsuario = usuario.IdUsuario,
                    Nombre = usuario.Nombre,
                    Email = usuario.Email,
                    Rol = (await _context.Roles.FindAsync(usuario.IdRol)).NombreRol
                };

                return CreatedAtAction(nameof(GetUsuario), new { id = usuarioDto.IdUsuario }, usuarioDto);
            }
        }

        // Modificamos un usuario
        [HttpPut("{usuarioId:int}")]
        public async Task<IActionResult> Modificar([FromBody] ModificarUsuarioDTO modificarUsuarioDto, [FromRoute] int usuarioId)
        {
            try
            {
                var usuarioExistente = await _context.Usuarios.FindAsync(usuarioId);

                if (usuarioExistente != null)
                {
                    if (!string.IsNullOrEmpty(modificarUsuarioDto.Nombre)) usuarioExistente.Nombre = modificarUsuarioDto.Nombre;
                    if (!string.IsNullOrEmpty(modificarUsuarioDto.Email)) usuarioExistente.Email = modificarUsuarioDto.Email;

                    //Aca agregue que encripte la contraseña cuando lo modifico ya que habia usuarios sin encriptar
                    if (!string.IsNullOrEmpty(modificarUsuarioDto.Contraseña))
                    {
                        using (var sha256 = SHA256.Create())
                        {
                            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(modificarUsuarioDto.Contraseña));
                            usuarioExistente.Contraseña = Convert.ToBase64String(hashBytes); // Almacenar la contraseña encriptada
                        }
                    }

                    if (modificarUsuarioDto.IdRol != 0) usuarioExistente.IdRol = modificarUsuarioDto.IdRol; 

                    _context.Usuarios.Update(usuarioExistente);
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

        // Eliminamos un usuario
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                return NotFound();
            }

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UsuarioExists(int id)
        {
            return _context.Usuarios.Any(e => e.IdUsuario == id);
        }


        [HttpPost("ValidarCredencial")]
        public async Task<IActionResult> ValidarCredencial([FromBody] UsuarioLoginDTO usuario)
        {
            
            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(usuario.contraseña));
                var contraseñaHash = Convert.ToBase64String(hashBytes);

                var usuarioLogin = await _context.Usuarios
                    .Where(x => x.Email.Equals(usuario.email) && x.Contraseña.Equals(contraseñaHash))
                    .Select(x => new LoginResponseDto
                    {
                        IdUsuario = x.IdUsuario,
                        Nombre = x.Nombre,
                        Email = x.Email,
                        IdRol = x.IdRol
                    })
                    .FirstOrDefaultAsync();

                if (usuarioLogin == null)
                {
                    return NotFound("Usuario o contraseña incorrectos");
                }

                return Ok(usuarioLogin);
            }
        }



       



    }



}

