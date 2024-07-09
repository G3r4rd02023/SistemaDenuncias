﻿using System.ComponentModel.DataAnnotations;

namespace UMA_SYSTEM.Frontend.Models
{
    public class Solicitud
    {
        public int Id { get; set; }

        [RegularExpression(@"[0-9]{4}[-]{1}[0-9]{4}[-]{1}[0-9]{5}", ErrorMessage = "Formato de DNI incorrecto.")]
        [MaxLength(20, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string DNI { get; set; } = null!;

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string NombreCompleto { get; set; } = null!;

        public int CantidadArboles { get; set; }

        [MaxLength(30, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string TerrenoTenencia { get; set; } = null!;

        [MaxLength(30, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Denominacion { get; set; } = null!;

        [MaxLength(200, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Observaciones { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public DateTime? FechaAprobacion { get; set; }

        public DateTime FechaVencimiento { get; set; }

        [MaxLength(60, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Aldea { get; set; }

        [MaxLength(60, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Barrio { get; set; }

        [MaxLength(60, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string? Caserio { get; set; }

        public Usuario? Usuario { get; set; }

        public int? IdUsuario { get; set; }
    }
}