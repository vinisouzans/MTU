using MTU.DTO.Pedido;

namespace MTU.Services.Interfaces
{
    public interface IPedidoService
    {
        Task<PedidoDTO> CriarPedidoAsync(PedidoCreateDTO dto);
        Task<PedidoDTO> ObterPorIdAsync(Guid id);
        Task<List<PedidoDTO>> ObterTodosAsync();
        Task<PedidoDTO> AtualizarStatusAsync(Guid id, string status);
        Task<bool> CancelarPedidoAsync(Guid id);
        Task<List<PedidoDTO>> ObterPorClienteAsync(Guid clienteId);
        Task<List<PedidoDTO>> ObterPedidosProntosParaEntregaAsync();
    }
}