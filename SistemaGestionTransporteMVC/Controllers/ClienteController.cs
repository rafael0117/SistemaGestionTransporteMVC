using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaGestionTransporteMVC.Models;

namespace SistemaGestionTransporteMVC.Controllers
{
    public class ClienteController : Controller
    {

        private readonly string apiUrlDestinos = "https://localhost:7252/api/DestinoAPI/";
        private readonly string apiUrlViaje = "https://localhost:7252/api/ViajeAPI/";


        public async Task<IActionResult> Index()
        {
            List<Destino> temporal = new List<Destino>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrlDestinos);
                HttpResponseMessage response = await client.GetAsync("getDestinos");
                string apiResponse = await response.Content.ReadAsStringAsync();
                temporal = JsonConvert.DeserializeObject<List<Destino>>(apiResponse);
            }
            return View(await Task.Run(() => temporal));
        }
        public async Task<IActionResult> IndexViaje(int? idDestino)
        {
            List<Viaje> viajes = new List<Viaje>();
            List<Destino> destinos = new List<Destino>();

            // Cargar destinos
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrlDestinos);
                HttpResponseMessage response = await client.GetAsync("getDestinos");
                string apiResponse = await response.Content.ReadAsStringAsync();
                destinos = JsonConvert.DeserializeObject<List<Destino>>(apiResponse);
            }

            // Cargar viajes
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrlViaje);
                HttpResponseMessage response;
                if (idDestino.HasValue)
                    response = await client.GetAsync($"getViajesPorDestino/{idDestino.Value}");
                else
                    response = await client.GetAsync("getViajes");

                string apiResponse = await response.Content.ReadAsStringAsync();
                viajes = JsonConvert.DeserializeObject<List<Viaje>>(apiResponse);
            }

            // Asignar el destino a cada viaje
            foreach (var viaje in viajes)
            {
                viaje.Destino = destinos.FirstOrDefault(d => d.IdDestino == viaje.IdDestino);
            }

            return View(viajes);
        }


    }
}
