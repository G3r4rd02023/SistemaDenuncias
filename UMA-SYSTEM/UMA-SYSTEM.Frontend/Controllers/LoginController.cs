﻿using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
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
        private readonly IParametroService _parametro;
        private readonly IServicioLista _lista;
        private readonly IMailService _mail;

        public LoginController(IHttpClientFactory httpClientFactory, IBitacoraService bitacoraService, IParametroService parametro,
            IServicioLista lista, IMailService mail)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://www.uma-valledeangeles.somee.com/");
            //_httpClient.BaseAddress = new Uri("https://localhost:7269/");
            _bitacora = bitacoraService;
            _parametro = parametro;
            _lista = lista;
            _mail = mail;
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
                usuario.EstadoUsuario = "Nuevo";
                usuario.FechaCreacion = DateTime.Now;
                usuario.FechaVencimiento = DateTime.Now.AddYears(int.Parse(await _parametro.ObtenerValor("Fecha de vencimiento de usuarios")));

                //usuario.FechaVencimiento = DateTime.Now.AddYears(2);
                var servicioToken = new ServicioToken();
                var token = await servicioToken.ConfirmarUsuario(usuario);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Login/Registro", content);

                if (response.IsSuccessStatusCode)
                {
                    Response respuesta = _mail.SendMail("Unidad Municipal Ambiental",
                       "departamentouma14@gmail.com",
                       $"<h1>UMA-Correo de confirmacion de Usuario</h1>",
                        $"Tu solicitud de usuario ha sido aprobada, para acceder al sistema, ingresa a UMA-SYSTEM" +
                              $"<p><a href =>Mas Detalles</a></p>" +
                              $"https://www.umasystem.somee.com"
                       );
                    TempData["Message"] = "Usuario registrado exitosamente, las instrucciones para su activación han sido enviadas a su correo";
                    var user = await _lista.GetUsuarioByEmail(User.Identity!.Name!);
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

                var email = Uri.EscapeDataString(model.Email);
                var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
                var usuarioJson = await userResponse.Content.ReadAsStringAsync();
                var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);

                var intentosPermitidos = (int.Parse(await _parametro.ObtenerValor("Intentos permitidos")));

                if (usuario!.NumeroIntentos <= intentosPermitidos)
                {
                    var response = await _httpClient.PostAsync("/api/Login/IniciarSesion", content);
                    if (response.IsSuccessStatusCode)
                    {
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
                        usuario.NumeroIntentos = 0;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                        {
                            ViewData["AlertMessage"] = "Este usuario ya tiene una sesion iniciada en el sistema, Desea cerrar la sesion existente?";
                            return RedirectToAction("CerrarSesion", "Login", new { correo = model.Email });
                        }
                        else
                        {
                            usuario.NumeroIntentos += 1;
                            ViewData["AlertMessage"] = "Usuario o clave incorrectos!!! numero de intentos: " + usuario.NumeroIntentos;
                        }
                    }
                }
            }
            else
            {
                ViewData["ErrorMessage"] = "Haz alcanzado el numero de intentos permitidos, comunicate con el administrador";
            }
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var email = Uri.EscapeDataString(User.Identity!.Name!);
            var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
            var usuarioJson = await userResponse.Content.ReadAsStringAsync();
            var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);

            var model = new LoginViewModel()
            {
                Email = usuario!.Email,
                Contraseña = usuario.Contraseña
            };

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("/api/Login/CerrarSesion", content);

            await _bitacora.AgregarRegistro(usuario!.Id, 1, "Finalizó sesión", "Fin de la sesión en el sistema");
            return RedirectToAction("IniciarSesion", "Login");
        }

        public async Task<IActionResult> CerrarSesion(string correo)
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            var email = Uri.EscapeDataString(correo);
            var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
            var usuarioJson = await userResponse.Content.ReadAsStringAsync();
            var usuario = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> CerrarSesion(Usuario usuario)
        {
            var model = new LoginViewModel()
            {
                Email = usuario!.Email,
                Contraseña = usuario.Contraseña
            };

            var json = JsonConvert.SerializeObject(model);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("/api/Login/CerrarSesion", content);

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