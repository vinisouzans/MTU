using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTU.Data;
using MTU.DTO.Moto;
using MTU.Events;
using MTU.Model;
using MTU.Services.Interfaces;

namespace MTU.Controllers
{
    [ApiController]
    [Route("api/motos")]
    public class MotoController : ControllerBase
    {
        private readonly IMotoService _service;

        public MotoController(IMotoService service)
        {
            _service = service;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MotoResponseDTO>> CriarMoto(MotoCreateDTO dto)
        {
            try
            {
                var moto = await _service.CriarMotoAsync(dto);
                return CreatedAtAction(nameof(GetMoto), new { id = moto.Id }, moto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpPut("{id}/placa")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MotoResponseDTO>> AtualizarPlaca(Guid id, [FromBody] MotoUpdatePlacaDTO dto)
        {
            try
            {
                var moto = await _service.AtualizarPlacaAsync(id, dto);
                return Ok(moto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RemoverMoto(Guid id)
        {
            try
            {
                await _service.RemoverMotoAsync(id);
                return Ok("Moto removida com sucesso");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MotoResponseDTO>> GetMoto(Guid id)
        {
            try
            {
                var moto = await _service.ObterMotoAsync(id);
                return Ok(moto);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { mensagem = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<MotoResponseDTO>>> GetMotos([FromQuery] string? placa)
        {
            var motos = await _service.ObterMotosAsync(placa);
            return Ok(motos);
        }
    }


}
