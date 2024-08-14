using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using UMA_SYSTEM.Frontend.Models;

namespace UMA_SYSTEM.Frontend.Controllers
{
    public class ImagenesController : Controller
    {

        private readonly HttpClient _httpClient;

        public ImagenesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://www.uma-valledeangeles.somee.com/");
        }

        public async Task<IActionResult> VerImagenes(int id)
        {
            var response = await _httpClient.GetAsync($"/api/Anexos/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var anexos = JsonConvert.DeserializeObject<IEnumerable<Anexo>>(content);
                return View("VerImagenes", anexos);
            }

            return View(new List<Anexo>());
        }


    }
}
