using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Text;
using UMA_SYSTEM.Frontend.Models;
using UMA_SYSTEM.Frontend.Services;

namespace UMA_SYSTEM.Frontend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServicioLista _lista;
        private readonly HttpClient _httpClient;

        public HomeController(ILogger<HomeController> logger, IServicioLista lista, IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _lista = lista;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7269/");
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> CrearDenuncia()
        {
            Denuncia denuncia = new()
            {
                Fecha = DateTime.Now,                
                Tipos = await _lista.GetListaTipos(),
                IdEstado = 1,
                Municipio = "Valle de Angeles"
            };
            return View(denuncia);
        }

        [HttpPost]
        public async Task<IActionResult> CrearDenuncia(Denuncia denuncia)
        {
            if (ModelState.IsValid)
            {
                denuncia.Fecha = DateTime.Now;
                denuncia.IdEstado = 1;
                denuncia.Municipio = "Valle de Anegeles";
                var json = JsonConvert.SerializeObject(denuncia);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Denuncias/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Denuncia creada exitosamente!!!";
                    return RedirectToAction("Privacy");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear la denuncia.");
                    TempData["ErrorMessage"] = "Ocurrió un error al intentar crear la denuncia!!!";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ModelErrors"] = string.Join("\n", errors);
            }
            
            denuncia.Tipos = await _lista.GetListaTipos();
            return View(denuncia);
        }

        public IActionResult CrearSolicitud()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearSolicitud(Solicitud solicitud)
        {
            if (ModelState.IsValid)
            {
                solicitud.IdUsuario = 1;
                solicitud.FechaSolicitud = DateTime.Now;
                solicitud.FechaVencimiento = DateTime.Now.AddDays(3);
                solicitud.FechaAprobacion = DateTime.Now.AddDays(3);
                var json = JsonConvert.SerializeObject(solicitud);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Solicitudes/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Solicitud creada exitosamente!!!";
                    return RedirectToAction("Privacy");
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

        public async Task<IActionResult> VerDenuncia(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Denuncias/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["ErrorMessage"] = "Error al obtener la denuncia.";
                return RedirectToAction("Index");
            }

            var jsonString = await response.Content.ReadAsStringAsync();
            var denuncia = JsonConvert.DeserializeObject<Denuncia>(jsonString);

            return View(denuncia);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
