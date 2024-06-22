using System.ComponentModel.DataAnnotations;

namespace UMA_SYSTEM.Backend.Models
{
    public class Parametro
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Nombre { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Valor { get; set; } = null!;

        public Usuario Usuario { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public DateTime FechaModificacion { get; set; }

    }
}
