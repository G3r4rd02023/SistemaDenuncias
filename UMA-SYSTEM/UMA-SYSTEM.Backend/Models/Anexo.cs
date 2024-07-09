using System.ComponentModel.DataAnnotations;

namespace UMA_SYSTEM.Backend.Models
{
    public class Anexo
    {
        public int Id { get; set; }

        [MaxLength(100, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]        
        public string? NombreArchivo { get; set; }
        public DateTime Fecha { get; set; }
        public string? URL { get; set; }
        public Denuncia? Denuncia { get; set; }
        public int IdDenuncia { get; set; }
    }
}
