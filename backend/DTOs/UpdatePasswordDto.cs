using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class UpdatePasswordDto
    {
        [Required]
        public string OldPassword { get; set; }
        [Required]
        public string NewPassword { get; set; }
    }
}
