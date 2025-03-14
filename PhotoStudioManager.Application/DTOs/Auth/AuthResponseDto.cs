namespace PhotoStudioManager.Application.DTOs.Auth;

public class AuthResponseDto
{
    public UserDto User { get; set; } = null!;
    public string Token { get; set; } = null!;
}
