using MTU.DTO.Locacao;
using MTU.Model;

namespace MTU.Services.Interfaces
{
    public interface ILocacaoService
    {
        Task<LocacaoResponseDTO> CriarLocacaoAsync(LocacaoCreateDTO dto);
        Task<LocacaoResponseDTO> ObterLocacaoAsync(Guid id);
        Task<SimularDevolucaoResponse> SimularDevolucaoAsync(SimularDevolucaoRequest request);
        Task<List<object>> ConsultarPorEntregadorAsync(string? cnpj, string? numeroCnh);
    }
}
