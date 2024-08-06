using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using System.Text;
using UMA_SYSTEM.Frontend.Models;

namespace UMA_SYSTEM.Frontend.Controllers
{
    public class SolicitudesICFController : Controller
    {

        private readonly HttpClient _httpClient;

        public SolicitudesICFController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7269/");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/SolicitudesICF");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var solicitudes = JsonConvert.DeserializeObject<IEnumerable<SolicitudICF>>(content);
                return View("Index", solicitudes);
            }

            return View(new List<Solicitud>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SolicitudICF solicitud)
        {
            if (ModelState.IsValid)
            {                              
                solicitud.Fecha = DateTime.Now;               
                var json = JsonConvert.SerializeObject(solicitud);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/SolicitudesICF/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Solicitud creada exitosamente!!!";
                    return RedirectToAction("Index");
                }
                else
                {
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
            var response = await _httpClient.GetAsync($"/api/SolicitudesICF/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Error al obtener la solicitud.";
                return RedirectToAction("Index");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var solicitud = JsonConvert.DeserializeObject<SolicitudICF>(jsonString);

            return View(solicitud);
        }

        public async Task<IActionResult> GenerarPdf(int id)
        {
            var response = await _httpClient.GetAsync($"/api/SolicitudesICF/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Error al obtener la solicitud.";
                return RedirectToAction("Index");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var solicitud = JsonConvert.DeserializeObject<SolicitudICF>(jsonString);

            return new ViewAsPdf("GenerarPdf", solicitud)
            {
                FileName = $"Solicitud {solicitud!.Id}.pdf",
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                PageSize = Rotativa.AspNetCore.Options.Size.A4
            };
        }
    }
}
