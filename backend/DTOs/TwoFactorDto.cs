using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class TwoFactorDto
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Provider { get; set; }
        [Required]
        public string Token { get; set; }
    }
}
