using System.ComponentModel.DataAnnotations;

namespace UMA_SYSTEM.Backend.Models
{
    public class LoginViewModel
    {
        [Required]
        [MaxLength(50)]
        public string Email { get; set; } = null!;

        [Required]
        [MaxLength(256)]
        public string Contraseña { get; set; } = null!;
    }
}
