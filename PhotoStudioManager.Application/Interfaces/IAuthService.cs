using PhotoStudioManager.Application.DTOs.Auth;

namespace PhotoStudioManager.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> ChangePasswordAsync(int userId, string currentPassword, string newPassword);
    Task<AuthResponseDto> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
}
