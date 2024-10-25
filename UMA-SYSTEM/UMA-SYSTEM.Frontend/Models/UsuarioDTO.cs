using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMA_SYSTEM.Frontend.Models
{
    public class UsuarioDTO
    {
        public int Id { get; set; }

        [RegularExpression(@"[0-9]{4}[-]{1}[0-9]{4}[-]{1}[0-9]{5}", ErrorMessage = "Formato de DNI incorrecto.")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string DNI { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Apellidos { get; set; } = null!;

        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Email { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaVencimiento { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string EstadoUsuario { get; set; } = null!;

        public int NumeroIntentos { get; set; }
        public int RolId { get; set; }

        public string FullName => $"{Nombre} {Apellidos}";

        [NotMapped]
        public IEnumerable<SelectListItem>? Roles { get; set; }
    }
}