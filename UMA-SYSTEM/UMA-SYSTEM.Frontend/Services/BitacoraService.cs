using Newtonsoft.Json;
using System;
using System.Text;
using UMA_SYSTEM.Frontend.Models;

namespace UMA_SYSTEM.Frontend.Services
{
    public class BitacoraService : IBitacoraService
    {

        private readonly HttpClient _httpClient;

        public BitacoraService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7269/");
        }

        public async Task<Bitacora> AgregarRegistro(int usuarioId, int objetoId,string accion, string descripcion)
        {
            Bitacora bitacora = new() 
            {
                Accion = accion,
                Descripcion = descripcion,
                UsuarioId = usuarioId,
                Fecha = DateTime.Now,
                ObjetoId = objetoId,
            };

            var json = JsonConvert.SerializeObject(bitacora);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _httpClient.PostAsync("/api/Bitacora", content);
            if (response.IsSuccessStatusCode)
            {
                return bitacora;
            }
            return bitacora;
        }


    }
}

