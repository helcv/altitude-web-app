using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class UpdateUserDto
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public DateOnly? DateOfBirth { get; set; }
    }
}
