using Microsoft.AspNetCore.Mvc;
using InventarioAPI.Models;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.EntityFrameworkCore;
using InventarioAPI.Data;
using System.Security.Cryptography;
using InventarioAPI.ModelsDTO;

namespace InventarioAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AutenticacionController : ControllerBase
    {
        private readonly InventarioContex _context;
        private readonly string clavesecreta;

        public AutenticacionController(InventarioContex context, IConfiguration config)
        {
            _context = context;
            clavesecreta = config.GetSection("settings").GetSection("clavesecreta").ToString();
        }

        [HttpPost]
        [Route("Validar")]
        public async Task<IActionResult> Validar([FromBody] UsuarioLoginDTO request)
        {
            
            if (request == null || string.IsNullOrEmpty(request.email) || string.IsNullOrEmpty(request.contraseña))
            {
                return BadRequest("El email y la contraseña son requeridos.");
            }

            using (var sha256 = SHA256.Create())
            {
                var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(request.contraseña));
                var hashedContraseña = Convert.ToBase64String(hashBytes);

                var usuarioLogin = await _context.Usuarios
                    .FirstOrDefaultAsync(x => x.Email.Equals(request.email) && x.Contraseña.Equals(hashedContraseña));

                if (usuarioLogin == null)
                {
                    return NotFound("Usuario o contraseña incorrectos");
                }
                else
                {
                    var keyBytes = Encoding.ASCII.GetBytes(clavesecreta);
                    var claims = new ClaimsIdentity();
                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, request.email));

                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = claims,
                        Expires = DateTime.UtcNow.AddMinutes(5),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
                    };

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var tokenConfig = tokenHandler.CreateToken(tokenDescriptor);

                    string tokencreado = tokenHandler.WriteToken(tokenConfig);

                    return Ok(new { token = tokencreado });
                }
            }
        }
    }
}
