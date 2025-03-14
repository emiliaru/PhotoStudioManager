using PhotoStudioManager.Core.Entities;

namespace PhotoStudioManager.Core.Interfaces;

public interface IPhotoSessionRepository : IRepository<PhotoSession>
{
    Task<IEnumerable<PhotoSession>> GetUpcomingSessionsAsync();
    Task<IEnumerable<PhotoSession>> GetSessionsByClientAsync(int clientId);
    Task<IEnumerable<PhotoSession>> GetSessionsByPhotographerAsync(int photographerId);
    Task<IEnumerable<PhotoSession>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate);
}
