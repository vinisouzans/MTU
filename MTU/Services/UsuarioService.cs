using Microsoft.EntityFrameworkCore;
using MTU.Data;
using MTU.DTO.Usuario;
using MTU.Model;
using MTU.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace MTU.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public UsuarioService(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<UsuarioResponseDTO> RegistrarAsync(UsuarioCreateDTO dto)
        {
            // Verificar se o email já existe
            var emailExistente = await _context.Usuarios.AnyAsync(u => u.Email == dto.Email);
            if (emailExistente)
                throw new InvalidOperationException("Email já cadastrado");

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

        public async Task<string> LoginAsync(UsuarioLoginDTO dto)
        {
            var usuario = await _context.Usuarios.SingleOrDefaultAsync(u => u.Email == dto.Email);
            if (usuario == null || !BCrypt.Net.BCrypt.Verify(dto.Senha, usuario.SenhaHash))
                throw new UnauthorizedAccessException("Usuário ou senha inválidos");

            // Gerar JWT
            return JwtService.GerarToken(usuario, _config);
        }
    }
}