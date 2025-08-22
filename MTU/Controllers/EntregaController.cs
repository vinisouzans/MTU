using Microsoft.AspNetCore.Mvc;
using MTU.DTO.Entrega;
using MTU.Services.Interfaces;

namespace MTU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EntregaController : ControllerBase
    {
        private readonly IEntregaService _entregaService;

        public EntregaController(IEntregaService entregaService)
        {
            _entregaService = entregaService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarEntrega([FromBody] EntregaCreateDTO dto)
        {
            try
            {
                var entrega = await _entregaService.CriarEntregaAsync(dto);
                return CreatedAtAction(nameof(ObterEntrega), new { id = entrega.Id }, entrega);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { erro = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListarEntregas()
        {
            try
            {
                var entregas = await _entregaService.ListarEntregasAsync();
                return Ok(entregas);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { erro = "Erro interno no servidor" });
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> ObterEntrega(Guid id)
        {
            try
            {
                var entrega = await _entregaService.ObterEntregaAsync(id);
                return Ok(entrega);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { erro = ex.Message });
            }
        }

        [HttpPatch("{id}/status")]
        public async Task<IActionResult> AtualizarStatus(Guid id, [FromBody] EntregaUpdateStatusDTO dto)
        {
            try
            {
                await _entregaService.AtualizarStatusAsync(id, dto);
                return Ok(new { mensagem = "Status atualizado com sucesso" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { erro = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoverEntrega(Guid id)
        {
            try
            {
                await _entregaService.RemoverEntregaAsync(id);
                return Ok(new { mensagem = "Entrega removida com sucesso" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { erro = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { erro = ex.Message });
            }
        }
    }
}