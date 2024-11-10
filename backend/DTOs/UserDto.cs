namespace backend.DTOs
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public  string ProfilePhotoUrl { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public bool IsTwoFactorEnabled { get; set; }
        public bool IsAuthWithGoogle { get; set; }
    }
}
