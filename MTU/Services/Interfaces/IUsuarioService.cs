using MTU.DTO.Usuario;

namespace MTU.Services.Interfaces
{
    public interface IUsuarioService
    {
        Task<UsuarioResponseDTO> RegistrarAsync(UsuarioCreateDTO dto);
        Task<string> LoginAsync(UsuarioLoginDTO dto);
    }
}
