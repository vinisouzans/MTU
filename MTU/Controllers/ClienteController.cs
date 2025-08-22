using Microsoft.AspNetCore.Mvc;
using MTU.DTO.Cliente;
using MTU.Services.Interfaces;

namespace MTU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteService _clienteService;

        public ClienteController(IClienteService clienteService)
        {
            _clienteService = clienteService;
        }

        [HttpPost]
        public async Task<IActionResult> CriarCliente([FromBody] ClienteCreateDTO dto)
        {
            try
            {
                var cliente = await _clienteService.CriarClienteAsync(dto);
                return CreatedAtAction(nameof(ObterPorId), new { id = cliente.Id }, cliente);
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
                var cliente = await _clienteService.ObterPorIdAsync(id);
                return Ok(cliente);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ObterTodos()
        {
            var clientes = await _clienteService.ObterTodosAsync();
            return Ok(clientes);
        }

        [HttpGet("email/{email}")]
        public async Task<IActionResult> ObterPorEmail(string email)
        {
            try
            {
                var cliente = await _clienteService.ObterPorEmailAsync(email);
                return Ok(cliente);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> AtualizarCliente(Guid id, [FromBody] ClienteUpdateDTO dto)
        {
            try
            {
                var cliente = await _clienteService.AtualizarClienteAsync(id, dto);
                return Ok(cliente);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> ExcluirCliente(Guid id)
        {
            try
            {
                var resultado = await _clienteService.ExcluirClienteAsync(id);
                return Ok(new { message = "Cliente excluído com sucesso" });
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