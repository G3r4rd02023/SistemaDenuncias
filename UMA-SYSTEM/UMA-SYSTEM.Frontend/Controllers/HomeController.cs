using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
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
        private readonly Cloudinary _cloudinary;
        private readonly IMailService _mail;

        public HomeController(ILogger<HomeController> logger, IServicioLista lista, IHttpClientFactory httpClientFactory, Cloudinary cloudinary, IMailService mail)
        {
            _logger = logger;
            _lista = lista;
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://www.uma-valledeangeles.somee.com/");
            _cloudinary = cloudinary;
            _mail = mail;
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
            var denuncia = new DenunciaAnonimaVM()
            {
                Fecha = DateTime.Now,
                Tipos = await _lista.GetListaTipos(),
                IdEstado = 3,
                Municipio = "Valle de Angeles",
                NumExpediente = await _lista.ObtenerCodigo()
            };

            return View(denuncia);
        }

        [HttpPost]
        public async Task<IActionResult> CrearDenuncia(DenunciaAnonimaVM denuncia, IFormFile file)
        {
            if (ModelState.IsValid)
            {
                var uploadParams = new ImageUploadParams()
                {
                    File = new FileDescription(file.FileName, file.OpenReadStream()),
                    AssetFolder = "umasystem"
                };

                var uploadResult = await _cloudinary.UploadAsync(uploadParams);
                var urlImagen = uploadResult.SecureUrl.ToString();

                denuncia.Fecha = DateTime.Now;
                denuncia.IdEstado = 3;
                denuncia.Municipio = "Valle de Angeles";
                denuncia.URLImagen = urlImagen;

                var user = await _lista.GetUsuarioByEmail("usuarioanonimo@gmail.com");

                var servicioToken = new ServicioToken();
                var token = await servicioToken.Autenticar(user);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(denuncia);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync("/api/Denuncias/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Denuncia creada exitosamente!!!";
                    Response respuesta = _mail.SendMail("Unidad Municipal Ambiental",
                       "departamentouma14@gmail.com",
                       $"UMA-Notificacion de Denuncia",
                        $"Se ha recibido una nueva denuncia, para mayor informaci�n, ingresa a UMA-SYSTEM" +
                              $"<p><a href =>Mas Detalles</a></p>" +
                              $"https://umasystem.gmail.com"
                       );
                    return RedirectToAction("Privacy");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear la denuncia.");
                    TempData["ErrorMessage"] = "Ocurri� un error al intentar crear la denuncia!!!";
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
            var solicitud = new Solicitud()
            {
                IdUsuario = 1,
                IdEstado = 1,
                FechaSolicitud = DateTime.Now,
                FechaVencimiento = DateTime.Now.AddDays(3),
                FechaAprobacion = DateTime.Now.AddDays(3)
            };
            return View(solicitud);
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

                var user = await _lista.GetUsuarioByEmail("usuarioanonimo@gmail.com");

                var servicioToken = new ServicioToken();
                var token = await servicioToken.Autenticar(user);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(solicitud);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Solicitudes/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Solicitud creada exitosamente!!!";
                    Response solicitante = _mail.SendMail("Unidad Municipal Ambiental",
                     solicitud.Correo!,
                     $"UMA-Notificacion de Solicitud",
                      $"Hemos recibido su solicitud, nuestro equipo se estar� comunicando con usted a la brevedad, para mayor informaci�n, ingresa a UMA-SYSTEM" +
                            $"<p><a href =>Mas Detalles</a></p>" +
                            $"https://umasystem.gmail.com"
                     );
                    Response admin_uma = _mail.SendMail("Unidad Municipal Ambiental",
                      "departamentouma14@gmail.com",
                      $"UMA-Notificacion de Solicitud",
                       $"Se ha recibido una solicitud de " + solicitud.NombreCompleto + " , su correo elecctronico es: " + solicitud.Correo + " " + solicitud.Telefono + ", para mayor informaci�n, ingresa a UMA-SYSTEM" +
                             $"<p><a href =>Mas Detalles</a></p>" +
                             $"https://umasystem.gmail.com"
                      );
                    return RedirectToAction("Privacy");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear la soliictud.");
                    TempData["ErrorMessage"] = "Ocurri� un error al intentar crear la soliictud!!!";
                }
            }
            else
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ModelErrors"] = string.Join("\n", errors);
            }

            return View(solicitud);
        }

        public IActionResult VerRequisitos()
        {
            return View();
        }

        public IActionResult CrearSolicitudICF()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearSolicitudICF(SolicitudICF solicitud)
        {
            if (ModelState.IsValid)
            {
                solicitud.IdEstado = 3;
                solicitud.Fecha = DateTime.Now;

                var user = await _lista.GetUsuarioByEmail("usuarioanonimo@gmail.com");

                var servicioToken = new ServicioToken();
                var token = await servicioToken.Autenticar(user);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var json = JsonConvert.SerializeObject(solicitud);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/SolicitudesICF/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Solicitud creada exitosamente!!!";
                    Response solicitante = _mail.SendMail("Unidad Municipal Ambiental",
                     solicitud.Correo!,
                     $"UMA-Notificacion de Solicitud",
                      $"Hemos recibido su solicitud, nuestro equipo se estar� comunicando con usted a la brevedad, para mayor informaci�n, ingresa a UMA-SYSTEM" +
                            $"<p><a href =>Mas Detalles</a></p>" +
                            $"https://umasystem.gmail.com"
                     );
                    Response admin_uma = _mail.SendMail("Unidad Municipal Ambiental",
                      "glanza007@gmail.com",
                      $"UMA-Notificacion de Solicitud ICF",
                       $"Se ha recibido una solicitud de " + solicitud.NombreCompleto + " , su correo elecctronico es :" + solicitud.Correo + " , para mayor informaci�n, ingresa a UMA-SYSTEM" +
                             $"<p><a href =>Mas Detalles</a></p>" +
                             $"https://umasystem.gmail.com"
                      );
                    return RedirectToAction("Privacy");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error al crear la soliictud.");
                    TempData["ErrorMessage"] = "Ocurri� un error al intentar crear la soliictud!!!";
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
            var user = await _lista.GetUsuarioByEmail("usuarioanonimo@gmail.com");

            var servicioToken = new ServicioToken();
            var token = await servicioToken.Autenticar(user);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
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