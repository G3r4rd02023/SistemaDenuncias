using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using UMA_SYSTEM.Frontend.Models;

namespace UMA_SYSTEM.Frontend.Services
{
    public class ServicioLista : IServicioLista
    {
        private readonly HttpClient _httpClient;

        public ServicioLista(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7269/");
        }

        public async Task<IEnumerable<SelectListItem>> GetListaEstados()
        {
            var response = await _httpClient.GetAsync("/api/Estados");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var estados = JsonConvert.DeserializeObject<IEnumerable<Estado>>(content);
                var listaEstados = estados!.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Descripcion
                }).ToList();

                listaEstados.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "Seleccione un Estado"
                });
                return listaEstados;
            }

            return [];
        }

        public async Task<IEnumerable<SelectListItem>> GetListaTipos()
        {
            var response = await _httpClient.GetAsync("/api/TiposDenuncia");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var tipos = JsonConvert.DeserializeObject<IEnumerable<Estado>>(content);
                var listaTipos = tipos!.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Descripcion
                }).ToList();

                listaTipos.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "Seleccione el tipo de denuncia"
                });
                return listaTipos;
            }

            return [];
        }
    }
}
