using backend.Entities;

namespace backend.Interfaces
{
    public interface IUserRepository
    {
        Task AddUserAsync(User user);
        void UpdateUser(User user);
        IQueryable<User> GetAllUsers();
        Task<bool> DeleteUserAsync(string id);
        Task<bool> SaveAllAsync();
    }
}
