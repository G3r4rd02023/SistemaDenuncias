using System.ComponentModel.DataAnnotations;

namespace UMA_SYSTEM.Frontend.Models
{
    public class Bitacora
    {
        public int Id { get; set; }

        public DateTime Fecha { get; set; }

        public Usuario? Usuario { get; set; } = null!;
        public int UsuarioId { get; set; }
        public Objeto? Objeto { get; set; } = null!;
        public int ObjetoId { get; set; }

        [MaxLength(50, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string Accion { get; set; } = null!;

        [MaxLength(200, ErrorMessage = "El campo {0} debe tener máximo {1} caractéres.")]
        public string Descripcion { get; set; } = null!;
    }
}
