using System.ComponentModel.DataAnnotations;

namespace UMA_SYSTEM.Frontend.Models
{
    public class CambiarPasswordVM
    {
        public int UserId { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Contraseña actual")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string OldPassword { get; set; } = null!;

        [DataType(DataType.Password)]
        [Display(Name = "Nueva contraseña")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
    ErrorMessage = "El campo {0} debe tener al menos una letra mayúscula, una minúscula, un número y un carácter especial.")]
        public string NewPassword { get; set; } = null!;

        [Compare("NewPassword", ErrorMessage = "La nueva contraseña y la confirmación no son iguales.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmación nueva contraseña")]
        [StringLength(20, MinimumLength = 8, ErrorMessage = "El campo {0} debe tener entre {2} y {1} carácteres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,20}$",
    ErrorMessage = "El campo {0} debe tener al menos una letra mayúscula, una minúscula, un número y un carácter especial.")]
        public string Confirmation { get; set; } = null!;
    }
}