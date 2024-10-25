using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using UMA_SYSTEM.Frontend.Models;
using UMA_SYSTEM.Frontend.Services;

namespace UMA_SYSTEM.Frontend.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IBitacoraService _bitacora;
        private readonly IServicioLista _lista;
        private readonly IMailService _mail;

        public UsuariosController(IHttpClientFactory httpClientFactory, IBitacoraService bitacoraService,
            IServicioLista lista, IMailService mail)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://www.uma-valledeangeles.somee.com/");
            //_httpClient.BaseAddress = new Uri("https://localhost:7269/");
            _bitacora = bitacoraService;
            _lista = lista;
            _mail = mail;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Usuarios");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var usuarios = JsonConvert.DeserializeObject<IEnumerable<Usuario>>(content);
                return View("Index", usuarios);
            }
            return View(new List<Usuario>());
        }

        public IActionResult Create()
        {
            var usuario = new Usuario()
            {
                EstadoUsuario = "Nuevo"
            };
            return View(usuario);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                usuario.FechaCreacion = DateTime.Now;
                usuario.FechaVencimiento = DateTime.Now.AddYears(2);
                usuario.EstadoUsuario = "Nuevo";
                usuario.RolId = 2;

                var user = await _lista.GetUsuarioByEmail(User.Identity!.Name!);
                var servicioToken = new ServicioToken();
                var token = await servicioToken.Autenticar(user);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Usuarios/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Message"] = "Usuario registrado exitosamente, las instrucciones para su activación han sido enviadas a su correo";
                    Response respuesta = _mail.SendMail("Unidad Municipal Ambiental",
                      usuario.Email,
                      $"UMA-Correo de confirmacion de Usuario",
                       $"Tu solicitud de creacion de usuario ha sido aprobada, para acceder al sistema, ingresa a UMA-SYSTEM" +
                             $"<p><a href =>Mas Detalles</a></p>" +
                             $"https://www.umasystem.somee.com/"
                      );

                    await _bitacora.AgregarRegistro(user!.Id, 2, "Insertó", "Registro de un nuevo usuario administrador");
                    return RedirectToAction("Index", "Usuarios");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al crear el usuario administrador!!!";
                }
            }

            return View(usuario);
        }

        public async Task<IActionResult> Edit()
        {
            var email = Uri.EscapeDataString(User.Identity!.Name!);
            var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
            var usuarioJson = await userResponse.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
            if (user == null)
            {
                return NotFound();
            }

            EditarUsuarioVM model = new()
            {
                Nombre = user.Nombre,
                Apellidos = user.Apellidos,
                DNI = user.DNI,
                Id = user.Id
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(EditarUsuarioVM model)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(model);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PutAsync($"/api/Usuarios/{model.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Usuario actualizado exitosamente!!!";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al actualizar usuario!!";
                }
            }

            return View(model);
        }

        public async Task<IActionResult> CambiarPassword()
        {
            var email = Uri.EscapeDataString(User.Identity!.Name!);
            var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
            var usuarioJson = await userResponse.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
            if (user == null)
            {
                return NotFound();
            }

            CambiarPasswordVM model = new()
            {
                UserId = user.Id,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarPassword(CambiarPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                var email = Uri.EscapeDataString(User.Identity!.Name!);
                var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");

                if (userResponse.IsSuccessStatusCode)
                {
                    var usuarioJson = await userResponse.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<Usuario>(usuarioJson);

                    if (user != null)
                    {
                        var changePasswordModel = new
                        {
                            model.UserId,
                            model.OldPassword,
                            model.NewPassword,
                            model.Confirmation
                        };

                        var content = new StringContent(JsonConvert.SerializeObject(changePasswordModel), Encoding.UTF8, "application/json");
                        var resultResponse = await _httpClient.PutAsync($"/api/Usuarios/CambiarPassword/{user.Email}", content);

                        if (resultResponse.IsSuccessStatusCode)
                        {
                            TempData["AlertMessage"] = "La contrasena se ha modificado exitosamente!";
                            await _bitacora.AgregarRegistro(model.UserId, 1, "Cambio contraseña", "El usuario cambió contraseña");
                            return RedirectToAction("Edit");
                        }
                        else
                        {
                            var errorResponse = await resultResponse.Content.ReadAsStringAsync();

                            ModelState.AddModelError(string.Empty, errorResponse);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al obtener el usuario.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Usuarios/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["AlertMessage"] = "Usuario eliminado exitosamente!!!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el usuario.";
                return RedirectToAction("Index");
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Usuarios/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Error al obtener el usuario.";
                return RedirectToAction("Index");
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<UsuarioDTO>(content);
                if (user == null)
                {
                    return NotFound();
                }

                var usuario = new UsuarioDTO()
                {
                    Apellidos = user.Apellidos,
                    FechaCreacion = user.FechaCreacion,
                    FechaVencimiento = user.FechaVencimiento,
                    DNI = user.DNI,
                    Email = user.Email,
                    EstadoUsuario = user.EstadoUsuario,
                    Nombre = user.Nombre,
                    NumeroIntentos = user.NumeroIntentos,
                    RolId = user.RolId,
                    Roles = await _lista.GetListaRoles()
                };

                return View(usuario);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Details(int id, UsuarioDTO usuario)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/Roles/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "El rol ha sido actualizado Exitosamente";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Ocurrio un error al actualizar el rol";
                    return RedirectToAction("Index");
                }
            }
            return View(usuario);
        }

        public IActionResult RecoverPassword()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RecoverPassword(RecoverPasswordVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _lista.GetUsuarioByEmail(model.Email);
                    if (user == null)
                    {
                        ModelState.AddModelError(string.Empty, "El email no corresponde a ningún usuario registrado.");
                        return View(model);
                    }

                    var servicioToken = new ServicioToken();
                    var token = await servicioToken.RecuperarPassword(user);
                    var callbackUrl = Url.Action("ResetPassword", "Usuarios", new { token = token, email = model.Email }, protocol: HttpContext.Request.Scheme);

                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                    ModelState.AddModelError(string.Empty, "Las instrucciones para recuperar su contraseña han sido enviadas a su correo electrónico.");
                    Response respuesta = _mail.SendMail("Unidad Municipal Ambiental",
                     model.Email,
                     $"UMA - Correo de recuperación de contraseña",
                     $"Tu solicitud de recuperación de contraseña ha sido aprobada, para acceder al sistema, inicia sesión en UMA-SYSTEM" +
                     $"<p><a href='{callbackUrl}'>Cambia tu contraseña aquí</a></p>"
                     );
                }
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error en la recuperación de contraseña para el usuario {model.Email}: {ex.Message}");
                throw;
            }
        }

        public IActionResult ResetPassword(string token, string email)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(email))
            {
                return BadRequest("Token o correo inválido.");
            }

            var model = new ResetPasswordVM { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM model)
        {
            if (ModelState.IsValid)
            {
                if (model.NewPassword != model.Confirmation)
                {
                    ModelState.AddModelError(string.Empty, "Las contraseñas no coinciden.");
                    return View(model);
                }

                var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{model.Email}");

                if (userResponse.IsSuccessStatusCode)
                {
                    var usuarioJson = await userResponse.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<Usuario>(usuarioJson);

                    if (user != null)
                    {
                        var changePasswordModel = new
                        {
                            model.Token,
                            model.Email,
                            model.NewPassword,
                            model.Confirmation
                        };

                        var content = new StringContent(JsonConvert.SerializeObject(changePasswordModel), Encoding.UTF8, "application/json");
                        var resultResponse = await _httpClient.PutAsync($"/api/Usuarios/ResetPassword/{model.Email}", content);

                        if (resultResponse.IsSuccessStatusCode)
                        {
                            TempData["AlertMessage"] = "La contrasena se ha modificado exitosamente!";
                            await _bitacora.AgregarRegistro(1, 1, "Cambio contraseña", "El usuario cambió contraseña");
                            return RedirectToAction("IniciarSesion", "Login");
                        }
                        else
                        {
                            var errorResponse = await resultResponse.Content.ReadAsStringAsync();

                            ModelState.AddModelError(string.Empty, errorResponse);
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Usuario no encontrado.");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al obtener el usuario.");
                }
            }
            return View(model);
        }
    }
}