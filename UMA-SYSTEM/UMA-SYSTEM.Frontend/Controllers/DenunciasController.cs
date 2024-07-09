﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using UMA_SYSTEM.Frontend.Models;
using UMA_SYSTEM.Frontend.Services;

namespace UMA_SYSTEM.Frontend.Controllers
{
    public class DenunciasController : Controller
    {

        private readonly HttpClient _httpClient;
        private readonly IServicioLista _lista;

        public DenunciasController(IHttpClientFactory httpClientFactory, IServicioLista lista)
        {
            _httpClient = httpClientFactory.CreateClient();
            _httpClient.BaseAddress = new Uri("https://localhost:7269/");
            _lista = lista;
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/api/Denuncias");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var denuncias = JsonConvert.DeserializeObject<IEnumerable<Denuncia>>(content);
                return View("Index", denuncias);
            }

            return View(new List<Denuncia>());
        }

        public async Task<IActionResult> Create()
        {
            Denuncia denuncia = new()
            {
                Estados = await _lista.GetListaEstados(),
                Tipos = await _lista.GetListaTipos()
            }; 
            return View(denuncia);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Denuncia denuncia)
        {
            if (ModelState.IsValid)
            {
                denuncia.Fecha = DateTime.Now;
                var json = JsonConvert.SerializeObject(denuncia);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("/api/Denuncias/", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["AlertMessage"] = "Denuncia creada exitosamente!!!";
                    return RedirectToAction("Index");
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
            denuncia.Estados = await _lista.GetListaEstados();
            denuncia.Tipos = await _lista.GetListaTipos();
            return View(denuncia);
        }
    }
}