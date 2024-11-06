using backend.Entities;

namespace backend.Authentication
{
    public interface IAuthTokenHandler
    {
        string CreateToken(User user);
    }
}
