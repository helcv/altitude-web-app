using Microsoft.AspNetCore.Identity;

namespace backend.Entities
{
    public class User : IdentityUser
    {
        public string Name { get; set; }
        public string LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string ProfilePhotoUrl { get; set; }
        public bool IsDeleted { get; set; } = false;
        public bool IsAuthWithGoogle { get; set; } = false;
    }
}
