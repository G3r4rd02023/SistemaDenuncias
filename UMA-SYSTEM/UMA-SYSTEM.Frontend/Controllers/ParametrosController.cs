using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UMA_SYSTEM.Frontend.Services;
using UMA_SYSTEM.Frontend.Models;
using System.Text;

namespace UMA_SYSTEM.Frontend.Controllers
{
    public class ParametrosController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly IBitacoraService _bitacora;

        public ParametrosController(IHttpClientFactory httpClientFactory, IBitacoraService bitacoraService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7269/");
            _bitacora = bitacoraService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Parametros");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var parametros = JsonConvert.DeserializeObject<IEnumerable<Parametro>>(content);
                return View("Index", parametros);
            }
            return View(new List<Parametro>());
        }

        public async Task<IActionResult> Create()
        {
            var email = Uri.EscapeDataString(User.Identity!.Name!);
            var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
            var usuarioJson = await userResponse.Content.ReadAsStringAsync();
            var user = JsonConvert.DeserializeObject<Usuario>(usuarioJson);

            Parametro parametro = new()
            {
                FechaCreacion = DateTime.Now,
                FechaModificacion = DateTime.Now,
                Usuario = user!,
                UsuarioId = user!.Id               
            };
            return View(parametro);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Parametro parametro)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(parametro);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Parametros/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Parametro creado exitosamente!!!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el parametro.");
                    TempData["ErrorMessage"] = "Ocurrió un error al intentar crear el parametro!!!";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ModelErrors"] = string.Join("\n", errors);
            }
            return View(parametro);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Parametros/{id}");
            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Error al obtener parametro.";
                return RedirectToAction("Index");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var parametro = JsonConvert.DeserializeObject<Parametro>(jsonString);

            return View(parametro);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Parametro parametro)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(parametro);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PutAsync($"/api/Parametros/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Parametro actualizado exitosamente!!!";
                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al actualizar el parametro.";
                    return RedirectToAction("Index");
                }
            }
            return View(parametro);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Parametros/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["AlertMessage"] = "Parametro eliminado exitosamente!!!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el parametro.";
                return RedirectToAction("Index");
            }
        }
    }
}
