using Newtonsoft.Json;
using System.Text;
using UMA_SYSTEM.Frontend.Models;

namespace UMA_SYSTEM.Frontend.Services
{
    public class ServicioToken
    {
        public async Task<string> Autenticar(Usuario usuario)
        {
            var cliente = new HttpClient();
            cliente.BaseAddress = new Uri("https://localhost:7269/");

            var credenciales = new Usuario()
            {
                DNI = usuario.DNI,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos,
                Email = usuario.Email,
                Contraseña = usuario.Contraseña,
                FechaCreacion = usuario.FechaCreacion,
                FechaVencimiento = usuario.FechaVencimiento,
                EstadoUsuario = usuario.EstadoUsuario,
                NumeroIntentos = usuario.NumeroIntentos,
                RolId = usuario.RolId
            };

            var content = new StringContent(JsonConvert.SerializeObject(credenciales), Encoding.UTF8, "application/json");
            var response = await cliente.PostAsync("api/Authentication/Validar", content);
            var json = await response.Content.ReadAsStringAsync();

            var resultado = JsonConvert.DeserializeObject<Credencial>(json);
            var token = resultado!.Token;
            return token;
        }

        public async Task<string> ConfirmarUsuario(Usuario usuario)
        {
            var cliente = new HttpClient();
            cliente.BaseAddress = new Uri("https://localhost:7269/");

            var credenciales = new Usuario()
            {
                DNI = usuario.DNI,
                Nombre = usuario.Nombre,
                Apellidos = usuario.Apellidos,
                Email = usuario.Email,
                Contraseña = usuario.Contraseña,
                FechaCreacion = usuario.FechaCreacion,
                FechaVencimiento = usuario.FechaVencimiento,
                EstadoUsuario = usuario.EstadoUsuario,
                NumeroIntentos = usuario.NumeroIntentos,
                RolId = usuario.RolId
            };

            var content = new StringContent(JsonConvert.SerializeObject(credenciales), Encoding.UTF8, "application/json");
            var response = await cliente.PostAsync("api/Authentication/ConfirmUser", content);
            var json = await response.Content.ReadAsStringAsync();

            var resultado = JsonConvert.DeserializeObject<Credencial>(json);
            var token = resultado!.Token;
            return token;
        }
    }
}