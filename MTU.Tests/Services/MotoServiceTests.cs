using Microsoft.EntityFrameworkCore;
using Moq;
using MTU.Data;
using MTU.DTO.Moto;
using MTU.Services;
using MTU.Services.Interfaces;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moq;
using MTU.Data;
using MTU.DTO.Moto;
using MTU.Events;
using MTU.Services;
using MTU.Services.Interfaces;
using Xunit;

namespace MTU.Tests.Services
{
    public class MotoServiceTests
    {
        private AppDbContext ObterContextoInMemory()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public async Task CriarMotoAsync_DeveCriarMoto_ComSucesso()
        {
            // Arrange
            var context = ObterContextoInMemory();
            var publisherMock = new Mock<IMotoPublisher>();
            var service = new MotoService(context, publisherMock.Object);

            var dto = new MotoCreateDTO
            {
                Ano = 2023,
                Modelo = "Honda CG 160",
                Placa = "ABC1D23"
            };

            // Act
            var result = await service.CriarMotoAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Placa, result.Placa);
            Assert.Equal(dto.Modelo, result.Modelo);
            Assert.Equal(dto.Ano, result.Ano);

            // Verifica se o publisher foi chamado
            publisherMock.Verify(p => p.Publicar("motos_cadastradas", It.IsAny<MotoCadastradaEvent>()), Times.Once);

            // Verifica se a moto foi realmente salva no contexto
            var motoNoDb = await context.Motos.FirstOrDefaultAsync();
            Assert.NotNull(motoNoDb);
            Assert.Equal(dto.Placa, motoNoDb.Placa);
        }

        [Fact]
        public async Task CriarMotoAsync_DeveFalhar_SePlacaDuplicada()
        {
            // Arrange
            var context = ObterContextoInMemory();
            context.Motos.Add(new MTU.Model.Moto { Placa = "ABC1D23", Modelo = "Modelo1", Ano = 2022 });
            await context.SaveChangesAsync();

            var publisherMock = new Mock<IMotoPublisher>();
            var service = new MotoService(context, publisherMock.Object);

            var dto = new MotoCreateDTO { Placa = "ABC1D23", Modelo = "Honda", Ano = 2023 };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.CriarMotoAsync(dto));
        }

        [Fact]
        public async Task RemoverMotoAsync_DeveFalhar_SeExistiremLocacoes()
        {
            // Arrange
            var context = ObterContextoInMemory();
            var moto = new MTU.Model.Moto { Placa = "XYZ1A23", Modelo = "Yamaha", Ano = 2020 };
            context.Motos.Add(moto);
            await context.SaveChangesAsync();

            context.Locacoes.Add(new MTU.Model.Locacao { MotoId = moto.Id, DataInicio = DateTime.UtcNow, DataTermino = DateTime.UtcNow.AddDays(1) });
            await context.SaveChangesAsync();

            var publisherMock = new Mock<IMotoPublisher>();
            var service = new MotoService(context, publisherMock.Object);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() => service.RemoverMotoAsync(moto.Id));
        }
    }
}
