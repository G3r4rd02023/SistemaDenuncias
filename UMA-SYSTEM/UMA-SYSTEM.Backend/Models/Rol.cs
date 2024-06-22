using System.ComponentModel.DataAnnotations;

namespace UMA_SYSTEM.Backend.Models
{
    public class Rol
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Descripcion { get; set; } = null!;
       
    }
}
