using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using UMA_SYSTEM.Frontend.Models;
using UMA_SYSTEM.Frontend.Services;

namespace UMA_SYSTEM.Frontend.Controllers
{
    public class RolesController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IBitacoraService _bitacora;

        public RolesController(IHttpClientFactory httpClientFactory, IBitacoraService bitacoraService)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://www.uma-valledeangeles.somee.com/");
            //_httpClient.BaseAddress = new Uri("https://localhost:7269/");
            _bitacora = bitacoraService;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Roles");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var roles = JsonConvert.DeserializeObject<IEnumerable<Rol>>(content);
                return View("Index", roles);
            }
            return View(new List<Rol>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Rol rol)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(rol);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Roles/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Rol creado exitosamente!!!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear el rol.");
                    TempData["ErrorMessage"] = "Ocurrió un error al intentar crear el rol!!!";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ModelErrors"] = string.Join("\n", errors);
            }
            return View(rol);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync($"/api/Roles/{id}");

            if (response.IsSuccessStatusCode)
            {
                TempData["AlertMessage"] = "Rol eliminado exitosamente!!!";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Error"] = "Error al eliminar el rol.";
                return RedirectToAction("Index");
            }
        }
    }
}