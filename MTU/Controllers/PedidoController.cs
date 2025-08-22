using Microsoft.AspNetCore.Mvc;
using MTU.DTO.Pedido;
using MTU.Services.Interfaces;

namespace MTU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PedidoController : ControllerBase
    {
        private readonly IPedidoService _pedidoService;

        public PedidoController(IPedidoService pedidoService)
        {
            _pedidoService = pedidoService;
        }

        [HttpGet("prontos-entrega")]
        public async Task<IActionResult> ObterPedidosProntosParaEntrega()
        {
            try
            {
                var pedidos = await _pedidoService.ObterPedidosProntosParaEntregaAsync();

                if (pedidos == null || !pedidos.Any())
                {
                    return NotFound(new { mensagem = "Não há pedidos prontos para entrega no momento." });
                }

                return Ok(pedidos);
            }
            catch (Exception ex)
            {                
                return StatusCode(500, new { erro = "Erro interno no servidor" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> CriarPedido([FromBody] PedidoCreateDTO dto)
        {
            try
            {
                var pedido = await _pedidoService.CriarPedidoAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = pedido.Id }, pedido);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterPorId(Guid id)
        {
            try
            {
                var pedido = await _pedidoService.ObterPorIdAsync(id);
                return Ok(pedido);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var pedidos = await _pedidoService.ObterTodosAsync();
            return Ok(pedidos);
        }

        [HttpGet("cliente/{clienteId}")]
        public async Task<IActionResult> ObterPorCliente(Guid clienteId)
        {
            try
            {
                var pedidos = await _pedidoService.ObterPorClienteAsync(clienteId);
                return Ok(pedidos);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(Guid id, [FromBody] string status)
        {
            try
            {
                var pedido = await _pedidoService.AtualizarStatusAsync(id, status);
                return Ok(pedido);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelarPedido(Guid id)
        {
            try
            {
                var resultado = await _pedidoService.CancelarPedidoAsync(id);
                return Ok(new { message = "Pedido cancelado com sucesso" });
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}