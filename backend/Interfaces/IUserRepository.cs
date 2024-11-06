using backend.Entities;

namespace backend.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        void UpdateUser(User user);
        Task<bool> SaveAllAsync();
    }
}
