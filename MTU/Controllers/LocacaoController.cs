using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTU.Data;
using MTU.DTO.Locacao;
using MTU.Model;
using MTU.Services.Interfaces;

namespace MTU.Controllers
{
    [ApiController]
    [Route("api/locacoes")]
    public class LocacaoController : ControllerBase
    {
        private readonly ILocacaoService _service;

        public LocacaoController(ILocacaoService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<ActionResult<LocacaoResponseDTO>> CriarLocacao(LocacaoCreateDTO dto)
        {
            try
            {
                var locacao = await _service.CriarLocacaoAsync(dto);
                return CreatedAtAction(nameof(GetLocacao), new { id = locacao.Id }, locacao);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LocacaoResponseDTO>> GetLocacao(Guid id)
        {
            try
            {
                var locacao = await _service.ObterLocacaoAsync(id);
                return Ok(locacao);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        [HttpPost("simular-devolucao")]
        public async Task<ActionResult<SimularDevolucaoResponse>> SimularDevolucao([FromBody] SimularDevolucaoRequest request)
        {
            try
            {
                var response = await _service.SimularDevolucaoAsync(request);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet("consultar-por-entregador")]
        public async Task<IActionResult> ConsultarPorEntregador([FromQuery] string? cnpj, [FromQuery] string? numeroCnh)
        {
            try
            {
                var result = await _service.ConsultarPorEntregadorAsync(cnpj, numeroCnh);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }
    }

}
