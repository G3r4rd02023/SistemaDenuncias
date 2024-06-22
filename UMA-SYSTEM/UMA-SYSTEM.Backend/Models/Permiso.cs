namespace UMA_SYSTEM.Backend.Models
{
    public class Permiso
    {
        public int Id { get; set; }
        public Objeto Objeto { get; set; } = null!;
        public bool PermisoInsercion { get; set; }
        public bool PermisoEliminacion { get; set; }
        public bool PermisoActualizacion { get; set; }
        public bool PermisoConsultar { get; set; }

    }
}
