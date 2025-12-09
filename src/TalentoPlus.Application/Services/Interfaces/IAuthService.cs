using TalentoPlus.Application.DTOs.Auth;

namespace TalentoPlus.Application.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
}
