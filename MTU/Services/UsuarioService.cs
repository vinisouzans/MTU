using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MTU.Data;
using MTU.DTO.Usuario;
using MTU.Model;
using MTU.Services.Interfaces;
using System.Text.RegularExpressions;

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
            if (!ValidarEmail(dto.Email))
                throw new InvalidOperationException("Formato de e-mail inválido");

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
            
            return JwtService.GerarToken(usuario, _config);
        }

        private bool ValidarEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}