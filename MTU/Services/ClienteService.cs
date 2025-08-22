using MTU.Data;
using MTU.DTO.Cliente;
using MTU.Model;
using MTU.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MTU.Services
{
    public class ClienteService : IClienteService
    {
        private readonly AppDbContext _context;

        public ClienteService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ClienteDTO> CriarClienteAsync(ClienteCreateDTO dto)
        {
            // Verificar se email já existe
            if (await _context.Clientes.AnyAsync(c => c.Email == dto.Email))
                throw new ArgumentException("Email já cadastrado");

            var cliente = new Cliente
            {
                Id = Guid.NewGuid(),
                Nome = dto.Nome,
                Email = dto.Email,
                Telefone = dto.Telefone,
                Endereco = dto.Endereco,
                DataCadastro = DateTime.UtcNow
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            return MapToDTO(cliente);
        }

        public async Task<ClienteDTO> ObterPorIdAsync(Guid id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                throw new ArgumentException("Cliente não encontrado");

            return MapToDTO(cliente);
        }

        public async Task<List<ClienteDTO>> ObterTodosAsync()
        {
            var clientes = await _context.Clientes
                .OrderBy(c => c.Nome)
                .ToListAsync();

            return clientes.Select(MapToDTO).ToList();
        }

        public async Task<ClienteDTO> AtualizarClienteAsync(Guid id, ClienteUpdateDTO dto)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
                throw new ArgumentException("Cliente não encontrado");

            // Verificar se novo email já existe (se foi alterado)
            if (!string.IsNullOrEmpty(dto.Email) && dto.Email != cliente.Email)
            {
                if (await _context.Clientes.AnyAsync(c => c.Email == dto.Email))
                    throw new ArgumentException("Email já cadastrado");
            }

            // Atualizar apenas os campos fornecidos
            if (!string.IsNullOrEmpty(dto.Nome)) cliente.Nome = dto.Nome;
            if (!string.IsNullOrEmpty(dto.Email)) cliente.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Telefone)) cliente.Telefone = dto.Telefone;
            if (!string.IsNullOrEmpty(dto.Endereco)) cliente.Endereco = dto.Endereco;

            await _context.SaveChangesAsync();

            return MapToDTO(cliente);
        }

        public async Task<bool> ExcluirClienteAsync(Guid id)
        {
            var cliente = await _context.Clientes
                .Include(c => c.Pedidos)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (cliente == null)
                throw new ArgumentException("Cliente não encontrado");

            // Verificar se há pedidos associados
            if (cliente.Pedidos.Any())
                throw new InvalidOperationException("Não é possível excluir cliente com pedidos associados");

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<ClienteDTO> ObterPorEmailAsync(string email)
        {
            var cliente = await _context.Clientes
                .FirstOrDefaultAsync(c => c.Email == email);

            if (cliente == null)
                throw new ArgumentException("Cliente não encontrado");

            return MapToDTO(cliente);
        }

        private ClienteDTO MapToDTO(Cliente cliente)
        {
            return new ClienteDTO
            {
                Id = cliente.Id,
                Nome = cliente.Nome,
                Email = cliente.Email,
                Telefone = cliente.Telefone,
                Endereco = cliente.Endereco,
                DataCadastro = cliente.DataCadastro
            };
        }
    }
}