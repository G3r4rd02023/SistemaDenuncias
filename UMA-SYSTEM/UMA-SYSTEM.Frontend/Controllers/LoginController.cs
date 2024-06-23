using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using System.Security.Claims;
using System.Text;
using UMA_SYSTEM.Frontend.Models;
using UMA_SYSTEM.Frontend.Services;

namespace UMA_SYSTEM.Frontend.Controllers
{
    public class LoginController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly IBitacoraService _bitacora;

        public LoginController(IHttpClientFactory httpClientFactory, IBitacoraService bitacoraService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7269/");
            _bitacora = bitacoraService;
        }

        public IActionResult Registro()
        {
            Usuario usuario = new()
            {
                EstadoUsuario = "Activo",
                RolId = 2,
            };
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Registro(Usuario usuario)
        {
            if (ModelState.IsValid)
            {

                usuario.RolId = 2;
                usuario.EstadoUsuario = "Activo";
                usuario.FechaCreacion = DateTime.Now;
                usuario.FechaVencimiento = DateTime.Now.AddYears(2);
                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Login/Registro", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Usuario registrado exitosamente!!!";
                    var email = Uri.EscapeDataString(usuario.Email);
                    var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
                    var usuarioJson = await userResponse.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
                    await _bitacora.AgregarRegistro(user!.Id, 2, "Insertó", "Registro de un nuevo usuario");
                    return RedirectToAction("IniciarSesion", "Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error, no se pudo crear el usuario");
                }
            }
            return View(usuario);
        }



        public IActionResult IniciarSesion()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IniciarSesion(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {

                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Login/IniciarSesion", content);
                if (response.IsSuccessStatusCode)
                {
                    var email = Uri.EscapeDataString(model.Email);
                    var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
                    var usuarioJson = await userResponse.Content.ReadAsStringAsync();
                    var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
                    var result = await _httpClient.GetAsync($"/api/Roles/{usuario!.RolId}");
                    var rolJson = await result.Content.ReadAsStringAsync();
                    var rol = JsonConvert.DeserializeObject<Rol>(rolJson);
                    var descripcion = rol!.Descripcion;

                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, model.Email),
                        new Claim(ClaimTypes.Role, descripcion),
                    };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
                    await _bitacora.AgregarRegistro(usuario.Id, 1, "Inicio sesión", "Inicio de sesión en el sistema");
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["AlertMessage"] = "Error al iniciar sesión!!!";
                }
            }

            return View(model);
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            var email = Uri.EscapeDataString(User.Identity!.Name!);
            var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
            var usuarioJson = await userResponse.Content.ReadAsStringAsync();
            var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
            await _bitacora.AgregarRegistro(usuario!.Id, 1, "Finalizó sesión", "Fin de la sesión en el sistema");
            return RedirectToAction("IniciarSesion", "Login");
        }

        public async Task<IActionResult> VerBitacora()
        {
            var response = await _httpClient.GetAsync("/api/Bitacora");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var bitacora = JsonConvert.DeserializeObject<IEnumerable<Bitacora>>(content);
                var email = Uri.EscapeDataString(User.Identity!.Name!);
                var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
                var usuarioJson = await userResponse.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
                await _bitacora.AgregarRegistro(usuario!.Id, 4, "Consultó", "Accedió a bitacora del sistema");
                return View("VerBitacora", bitacora);
            }
            return View(new List<Bitacora>());
        }
    }
}
