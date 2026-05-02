using Entities.Models;

namespace Logic.Interfaces
{
    public interface IJwtTokenService
    {
        string GenerateToken(Organizer organizer);
        DateTime GetExpiryUtc();
    }
}
