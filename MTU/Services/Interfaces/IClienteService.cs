using MTU.DTO.Cliente;

namespace MTU.Services.Interfaces
{
    public interface IClienteService
    {
        Task<ClienteDTO> CriarClienteAsync(ClienteCreateDTO dto);
        Task<ClienteDTO> ObterPorIdAsync(Guid id);
        Task<List<ClienteDTO>> ObterTodosAsync();
        Task<ClienteDTO> AtualizarClienteAsync(Guid id, ClienteUpdateDTO dto);
        Task<bool> ExcluirClienteAsync(Guid id);
        Task<ClienteDTO> ObterPorEmailAsync(string email);
    }
}
