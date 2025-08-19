using Microsoft.AspNetCore.Mvc;
using MTU.Data;
using MTU.DTO.Usuario;
using MTU.Model;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;
using MTU.Services;


namespace MTU.Controllers
{
    [ApiController]
    [Route("api/usuarios")]
    public class UsuarioController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UsuarioController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult<UsuarioResponseDTO>> Register(UsuarioCreateDTO dto)
        {
            var usuario = new Usuario
            {
                Nome = dto.Nome,
                Email = dto.Email,
                SenhaHash = BCrypt.Net.BCrypt.HashPassword(dto.Senha),
                EhAdmin = dto.EhAdmin
            };

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();

            return new UsuarioResponseDTO
            {
                Id = usuario.Id,
                Nome = usuario.Nome,
                Email = usuario.Email,
                EhAdmin = usuario.EhAdmin
            };
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UsuarioLoginDTO dto)
        {
            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
                return Unauthorized("Usuário ou senha inválidos");

            // Gerar JWT
            var token = JwtService.GerarToken(usuario, _config);
            return Ok(token);
        }
    }

}
