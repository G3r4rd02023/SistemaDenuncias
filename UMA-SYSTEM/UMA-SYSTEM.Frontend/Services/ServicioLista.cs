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

        public async Task<IEnumerable<SelectListItem>> GetListaRoles()
        {
            var response = await _httpClient.GetAsync("/api/Roles");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var roles = JsonConvert.DeserializeObject<IEnumerable<Rol>>(content);
                var listaRoles = roles!.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = c.Descripcion
                }).ToList();

                listaRoles.Insert(0, new SelectListItem
                {
                    Value = "",
                    Text = "Seleccione el rol"
                });
                return listaRoles;
            }

            return [];
        }

        public async Task<string> ObtenerCodigo()
        {
            var response = await _httpClient.GetAsync("/api/Denuncias");
            var content = await response.Content.ReadAsStringAsync();
            var denuncias = JsonConvert.DeserializeObject<IEnumerable<Denuncia>>(content);
            var lastCodigo = denuncias!.OrderByDescending(x => x.Id).Select(c => c.NumExpediente).FirstOrDefault();
            int lastNumber = 0;
            if (!string.IsNullOrEmpty(lastCodigo) && lastCodigo.Length > 2)
            {
                int.TryParse(lastCodigo.Substring(2), out lastNumber);
            }
            var codigo = $"E-{(lastNumber + 1).ToString("D5")}";
            return codigo;
        }
    }
}
