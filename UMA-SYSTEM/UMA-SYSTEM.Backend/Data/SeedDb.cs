using Microsoft.EntityFrameworkCore;
using UMA_SYSTEM.Backend.Models;

namespace UMA_SYSTEM.Backend.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await ValidarRolesAsync("Administrador");
            await ValidarRolesAsync("Usuario");
            await ValidarObjetosAsync("Login", "Pantalla de Login", "Formulario");
            await ValidarObjetosAsync("Registro", "Pantalla de registro", "Formulario");
            await ValidarObjetosAsync("Home", "Pantalla de inicio", "Pantalla");
            await ValidarObjetosAsync("Bitacora", "Pantalla de bitacora", "Pantalla");
            var rol = _context.Roles.FirstOrDefault();
            await ValidarUsuariosAsync("0801-1997-12345","SUPER","ADMIN", "superadmin@gmail.com","123456", "Activo", rol!);
        }

        private async Task<Objeto> ValidarObjetosAsync(string nombre, string descripcion, string tipo)
        {
            var objetoExistente = await _context.Objetos.FirstOrDefaultAsync(o => o.Nombre == nombre);
            if (objetoExistente != null)
            {
                return objetoExistente;
            }

            Objeto objeto = new()
            {
                Nombre = nombre,
                Descripcion = descripcion,
                Tipo = tipo
            };

            _context.Objetos.Add(objeto);
            await _context.SaveChangesAsync();
            return objeto;
        }

        private async Task<Rol> ValidarRolesAsync(string nombreRol)
        {
            var rolExistente = await _context.Roles.FirstOrDefaultAsync(r => r.Descripcion == nombreRol);
            if (rolExistente != null) 
            {
                return rolExistente;
            }

            Rol rol = new()
            {
                Descripcion = nombreRol,
            };

            _context.Roles.Add(rol);
            await _context.SaveChangesAsync();
            return rol;
        }

        private async Task<Usuario> ValidarUsuariosAsync(string dni, string nombre, string apellidos, string correo,string clave, string estado, Rol rolUsuario)
        {
            var usuarioExistente = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == correo);
            if (usuarioExistente != null)
            {
                return usuarioExistente;
            }

            Usuario usuario = new()
            {
                DNI = dni,
                Nombre = nombre,
                Apellidos = apellidos,
                Email = correo,
                Contraseña = clave,
                EstadoUsuario = estado,
                Rol = rolUsuario,
                FechaCreacion = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddYears(2)
            };

            usuario.Contraseña = BCrypt.Net.BCrypt.HashPassword(usuario.Contraseña);

            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
            return usuario;
        }
    }
}
