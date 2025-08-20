using MTU.DTO.Moto;
using MTU.Model;

namespace MTU.Services.Interfaces
{
    public interface IMotoService
    {
        Task<MotoResponseDTO> CriarMotoAsync(MotoCreateDTO dto);
        Task<MotoResponseDTO> AtualizarPlacaAsync(Guid id, MotoUpdatePlacaDTO dto);
        Task RemoverMotoAsync(Guid id);
        Task<MotoResponseDTO> ObterMotoAsync(Guid id);
        Task<List<MotoResponseDTO>> ObterMotosAsync(string? placa = null);
    }
}
