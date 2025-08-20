using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MTU.Data;
using MTU.DTO.Entregador;
using MTU.Model;
using MTU.Services.Interfaces;

namespace MTU.Controllers
{
    [ApiController]
    [Route("api/entregadores")]
    public class EntregadorController : ControllerBase
    {
        private readonly IEntregadorService _service;
        private readonly IWebHostEnvironment _env;

        public EntregadorController(IEntregadorService service, IWebHostEnvironment env)
        {
            _service = service;
            _env = env;
        }

        [HttpPost]
        public async Task<ActionResult<EntregadorResponseDTO>> CriarEntregador(EntregadorCreateDTO dto)
        {
            try
            {
                var result = await _service.CriarEntregadorAsync(dto);
                return CreatedAtAction(nameof(GetEntregador), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<EntregadorResponseDTO>> AtualizarEntregador(Guid id, EntregadorUpdateDTO dto)
        {
            try
            {
                var result = await _service.AtualizarEntregadorAsync(id, dto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EntregadorResponseDTO>> GetEntregador(Guid id)
        {
            try
            {
                var result = await _service.ObterEntregadorAsync(id);
                return Ok(result);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<ActionResult<List<EntregadorResponseDTO>>> GetEntregadores()
        {
            var result = await _service.ObterTodosEntregadoresAsync();
            return Ok(result);
        }

        [HttpPost("{id}/upload-cnh")]
        public async Task<IActionResult> UploadCNH(Guid id, IFormFile arquivo)
        {
            try
            {
                var caminho = await _service.UploadCNHAsync(id, arquivo, _env.WebRootPath, Request);
                return Ok(new { mensagem = "Upload realizado com sucesso!", caminho });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }


}
