using Entities.Dtos.Auth;

namespace Logic.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(OrganizerRegisterDto dto, CancellationToken ct);
        Task<AuthResponseDto> LoginAsync(OrganizerLoginDto dto, CancellationToken ct);
    }
}
