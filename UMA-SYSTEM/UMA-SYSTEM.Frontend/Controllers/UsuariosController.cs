using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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

        public UsuariosController(IHttpClientFactory httpClientFactory, IBitacoraService bitacoraService, IServicioLista lista)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://www.uma-valledeangeles.somee.com/");
            _bitacora = bitacoraService;
            _lista = lista;
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
                EstadoUsuario = "Activo"
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
                usuario.EstadoUsuario = "Activo";
                usuario.RolId = 2;
                var json = JsonConvert.SerializeObject(usuario);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Usuarios/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Usuario creado exitosamente!!!";
                    var email = Uri.EscapeDataString(usuario.Email);
                    var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
                    var usuarioJson = await userResponse.Content.ReadAsStringAsync();
                    var user = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
                    await _bitacora.AgregarRegistro(user!.Id, 2, "Insertó", "Registro de un nuevo usuario administrador");
                    return RedirectToAction("Index");
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
                            UserId = model.UserId,
                            OldPassword = model.OldPassword,
                            NewPassword = model.NewPassword,
                            Confirmation = model.Confirmation
                        };

                        var content = new StringContent(JsonConvert.SerializeObject(changePasswordModel), Encoding.UTF8, "application/json");
                        var resultResponse = await _httpClient.PutAsync($"/api/Usuarios/CambiarPassword/{model.UserId}", content);

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
                var user = JsonConvert.DeserializeObject<Usuario>(content);
                if (user == null)
                {
                    return NotFound();
                }

                var usuario = new Usuario()
                {
                    Apellidos = user.Apellidos,
                    Contraseña = user.Contraseña,
                    FechaCreacion = user.FechaCreacion,
                    FechaVencimiento = user.FechaVencimiento,
                    DNI = user.DNI,
                    Email = user.Email,
                    EstadoUsuario = user.EstadoUsuario,
                    Nombre  = user.Nombre,
                    NumeroIntentos = user.NumeroIntentos,
                    Rol = user.Rol,
                    RolId = user.RolId,
                    Roles = await _lista.GetListaRoles()
                };

                return View(usuario);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Details(int id, Usuario usuario)
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
    }
}
