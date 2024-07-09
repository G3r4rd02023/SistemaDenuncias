﻿using System.ComponentModel.DataAnnotations;

namespace UMA_SYSTEM.Frontend.Models
{
    public class Estado
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        [Required(ErrorMessage = "El campo {0} es obligatorio.")]
        public string Descripcion { get; set; } = null!;
    }
}
