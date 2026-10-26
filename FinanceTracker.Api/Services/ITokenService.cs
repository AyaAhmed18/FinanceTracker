using FinanceTracker.Api.Entities;

namespace FinanceTracker.Api.Services
{
    public interface ITokenService
    {
        (string Token, DateTime ExpiresAt) CreateToken(User user);

    }
}
