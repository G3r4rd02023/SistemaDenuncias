using Microsoft.EntityFrameworkCore;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Anexo> Anexos { get; set; }
        public DbSet<Bitacora> Bitacora { get; set; }                           
        public DbSet<Denuncia> Denuncias { get; set; }
        public DbSet<Estado> Estados { get; set; }
        public DbSet<Objeto> Objetos { get; set; }
        public DbSet<Parametro> Parametros { get; set; }
        public DbSet<Permiso> Permisos { get; set; }
        public DbSet<Rol> Roles { get; set; }
        public DbSet<Solicitud> Solicitudes { get; set; }
        public DbSet<SolicitudICF> SolicitudesICF { get; set; }
        public DbSet<TipoDenuncia> TiposDenuncia { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
    }
}
