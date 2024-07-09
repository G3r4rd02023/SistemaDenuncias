using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UMA_SYSTEM.Frontend.Models
{
    public class Denuncia
    {
        public int Id { get; set; }

        [MaxLength(15, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string NumExpediente { get; set; } = null!;

        public DateTime Fecha { get; set; }

        [MaxLength(60, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? PuntoReferencia { get; set; }

        [MaxLength(200, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? DescripcionHechos { get; set; }

        [MaxLength(60, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Municipio { get; set; }

        [MaxLength(60, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Aldea { get; set; }

        [MaxLength(60, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Barrio { get; set; }

        [MaxLength(60, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Caserio { get; set; }
        public TipoDenuncia? TipoDenuncia { get; set; }
        public int IdTipoDenuncia { get; set; }
        public Estado? Estado { get; set; }
        public int IdEstado { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Estados { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? Tipos { get; set; }

    }
}
