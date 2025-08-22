using MTU.DTO.Entrega;

namespace MTU.Services.Interfaces
{
    public interface IEntregaService
    {
        Task<EntregaResponseDTO> CriarEntregaAsync(EntregaCreateDTO dto);
        Task<IEnumerable<EntregaResponseDTO>> ListarEntregasAsync();
        Task<EntregaResponseDTO> ObterEntregaAsync(Guid id);
        Task AtualizarStatusAsync(Guid id, EntregaUpdateStatusDTO dto);
        Task RemoverEntregaAsync(Guid id);
        Task<bool> EntregadorTemLocacaoAtivaAsync(Guid entregadorId);
    }
}
