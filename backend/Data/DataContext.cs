using backend.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace backend.Data
{
    public class DataContext : IdentityDbContext<User, Role, string>
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }
    }
}
