using MTU.DTO.Entregador;
using MTU.Model;

namespace MTU.Services.Interfaces
{
    public interface IEntregadorService
    {
        Task<EntregadorResponseDTO> CriarEntregadorAsync(EntregadorCreateDTO dto);
        Task<EntregadorResponseDTO> AtualizarEntregadorAsync(Guid id, EntregadorUpdateDTO dto);
        Task<EntregadorResponseDTO> ObterEntregadorAsync(Guid id);
        Task<List<EntregadorResponseDTO>> ObterTodosEntregadoresAsync();
        Task<string> UploadCNHAsync(Guid id, IFormFile arquivo, string webRootPath, HttpRequest request);
    }
}
