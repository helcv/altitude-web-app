using System.ComponentModel.DataAnnotations;

namespace backend.DTOs
{
    public class EnableTwoFactorDto
    {
        [Required]
        public bool? IsEnabled { get; set; } 
    }
}
