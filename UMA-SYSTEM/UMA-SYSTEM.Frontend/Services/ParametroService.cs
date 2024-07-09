using Newtonsoft.Json;
using UMA_SYSTEM.Frontend.Models;

namespace UMA_SYSTEM.Frontend.Services
{
    public class ParametroService : IParametroService
    {
        private readonly HttpClient _httpClient;

        public ParametroService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7269/");
        }

        public async Task<string> ObtenerValor(string nombre)
        {
            var response = await _httpClient.GetAsync("/api/Parametros");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var parametros = JsonConvert.DeserializeObject<IEnumerable<Parametro>>(content);
                var parametro = parametros!.FirstOrDefault(p => p.Nombre == nombre);
                var valor = parametro!.Valor;
                return valor;
            }
            return "Parametro no encontrado";
        }

       
    }
}
