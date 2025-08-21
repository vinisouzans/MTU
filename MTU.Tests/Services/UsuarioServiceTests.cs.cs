using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using MTU.Data;
using MTU.DTO.Usuario;
using MTU.Model;
using MTU.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace MTU.Tests.Services
{
    public class UsuarioServiceTests
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;
        private readonly UsuarioService _usuarioService;

        public UsuarioServiceTests()
        {
            // Db em memória
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            // Configuração de JWT para testes
            var inMemorySettings = new Dictionary<string, string?>
        {
            { "Jwt:Key", "UnitTestSecretKey_12345678901234567890" }, // 32+ chars
            { "Jwt:Issuer", "MTUApi" },
            { "Jwt:Audience", "MTUApiUsers" }
            // adicione outras chaves que seu JwtService use, se houver
        };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Instancie o service do mesmo jeito que a app faz
            _usuarioService = new UsuarioService(_context, _config);
        }

        [Fact]
        public async Task Registrar_DeveCriarUsuario_ComSucesso()
        {
            // Arrange
            var dto = new UsuarioCreateDTO
            {
                Nome = "Vinicius",
                Email = "teste@teste.com",
                Senha = "123456",
                EhAdmin = true
            };

            // Act
            var result = await _usuarioService.RegistrarAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Nome, result.Nome);
            Assert.Equal(dto.Email, result.Email);
            Assert.True(result.EhAdmin);
        }

        [Fact]
        public async Task Login_DeveRetornarToken_QuandoCredenciaisValidas()
        {
            // Arrange
            var dto = new UsuarioCreateDTO
            {
                Nome = "User",
                Email = "login@teste.com",                
                Senha = "senha123",
                EhAdmin = true
            };

            await _usuarioService.RegistrarAsync(dto);

            var loginDto = new UsuarioLoginDTO
            {
                Email = "login@teste.com",
                Senha = "senha123"
            };

            // Act
            var token = await _usuarioService.LoginAsync(loginDto);

            // Assert
            Assert.False(string.IsNullOrEmpty(token));
        }


        [Fact]
        public async Task Login_DeveFalhar_QuandoSenhaIncorreta()
        {
            // Arrange
            var dto = new UsuarioCreateDTO
            {
                Nome = "User2",
                Email = "falha@teste.com",
                Senha = "correta",
                EhAdmin = false
            };

            await _usuarioService.RegistrarAsync(dto);

            var loginDto = new UsuarioLoginDTO
            {
                Email = "falha@teste.com",
                Senha = "errada"
            };

            // Act
            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                async () => await _usuarioService.LoginAsync(loginDto)
            );

            // Assert
            Assert.Equal("Usuário ou senha inválidos", exception.Message);
        }
    }
}
