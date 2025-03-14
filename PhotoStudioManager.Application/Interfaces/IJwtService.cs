using PhotoStudioManager.Core.Entities;

namespace PhotoStudioManager.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user);
}
