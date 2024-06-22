using Microsoft.EntityFrameworkCore;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Bitacora> Bitacora { get; set; }

        public DbSet<Rol> Roles { get; set; }

        public DbSet<Objeto> Objetos { get; set; }

        public DbSet<Parametro> Parametros { get; set; }

        public DbSet<Permiso> Permisos { get; set; }
    }
}
