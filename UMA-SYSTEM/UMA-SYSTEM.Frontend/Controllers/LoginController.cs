using Microsoft.AspNetCore.Mvc;
using System.Text;
using UMA_SYSTEM.Frontend.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace UMA_SYSTEM.Frontend.Controllers
{
    public class LoginController : Controller
    {

        private readonly HttpClient _httpClient;
      

        public LoginController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7269/");
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

                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewData["AlertMessage"] = "Error al iniciar sesion!!!";
                }
            }

            return View(model);
        }

        public async Task<IActionResult> CerrarSesion()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("IniciarSesion", "Login");
        }
    }
}
