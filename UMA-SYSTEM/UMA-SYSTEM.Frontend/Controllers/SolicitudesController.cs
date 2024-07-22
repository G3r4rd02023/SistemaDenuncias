using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Text;
using UMA_SYSTEM.Frontend.Models;

namespace UMA_SYSTEM.Frontend.Controllers
{
    public class SolicitudesController : Controller
    {
        private readonly HttpClient _httpClient;


        public SolicitudesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7269/");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Solicitudes");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var solicitudes = JsonConvert.DeserializeObject<IEnumerable<Solicitud>>(content);
                return View("Index", solicitudes);
            }

            return View(new List<Solicitud>());
        }

        public IActionResult Create()
        {            
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                var email = Uri.EscapeDataString(User.Identity!.Name!);
                var userResponse = await _httpClient.GetAsync($"/api/Usuarios/email/{email}");
                var usuarioJson = await userResponse.Content.ReadAsStringAsync();
                var user = JsonConvert.DeserializeObject<Usuario>(usuarioJson);
                solicitud.Usuario = user!;
                solicitud.IdUsuario = user!.Id;
                solicitud.FechaSolicitud = DateTime.Now;
                solicitud.FechaVencimiento = DateTime.Now.AddDays(3);
                solicitud.FechaAprobacion = DateTime.Now.AddDays(3);
                var json = JsonConvert.SerializeObject(solicitud);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Solicitudes/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Solicitud creada exitosamente!!!";
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear la soliictud.");
                    TempData["ErrorMessage"] = "Ocurrió un error al intentar crear la soliictud!!!";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ModelErrors"] = string.Join("\n", errors);
            }
            
            return View(solicitud);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Solicitudes/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Error al obtener la solicitud.";
                return RedirectToAction("Index");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var solicitud = JsonConvert.DeserializeObject<Solicitud>(jsonString);

            return View(solicitud);
        }

        public async Task<IActionResult> GenerarPdf(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Solicitudes/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Error al obtener la solicitud.";
                return RedirectToAction("Index");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var solicitud = JsonConvert.DeserializeObject<Solicitud>(jsonString);

            return new ViewAsPdf("GenerarPdf", solicitud)
            {
                FileName = $"Solicitud {solicitud!.Id}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }
    }
}
