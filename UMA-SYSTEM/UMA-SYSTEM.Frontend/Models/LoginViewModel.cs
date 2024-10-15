using System.ComponentModel.DataAnnotations;

namespace UMA_SYSTEM.Frontend.Models
{
    public class LoginViewModel
    {
        [Required]
        [MaxLength(50)]
        public string Email { get; set; } = null!;

        [Required]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
        public string Contraseña { get; set; } = null!;
    }
}