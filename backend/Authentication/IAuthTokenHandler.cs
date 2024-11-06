using backend.Entities;

namespace backend.Authentication
{
    public interface IAuthTokenHandler
    {
        Task<string> CreateToken(User user);
    }
}
