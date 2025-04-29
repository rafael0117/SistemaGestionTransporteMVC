using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaGestionTransporteMVC.Models;
using System.Text;

namespace SistemaGestionTransporteMVC.Controllers
{
    public class DestinoController : Controller
    {
        private readonly string apiUrl = "https://localhost:7252/api/DestinoAPI/";

        public async Task<IActionResult> Index()
        {
            List<Destino> temporal = new List<Destino>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync("getDestinos");
                string apiResponse = await response.Content.ReadAsStringAsync();
                temporal = JsonConvert.DeserializeObject<List<Destino>>(apiResponse);
            }
            return View(await Task.Run(() => temporal));
        }

        public async Task<IActionResult> Details(int id)
        {
            Destino destino = new Destino();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getDestino/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    destino = JsonConvert.DeserializeObject<Destino>(apiResponse);
                }
                else
                {
                    return NotFound();
                }
            }
            return View(await Task.Run(() => destino));
        }

        public async Task<IActionResult> Create()
        {
            return View(await Task.Run(() => new Destino()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Destino reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("insertDestino", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }

        public async Task<IActionResult> Edit(int id)
        {
            Destino destino = new Destino();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getDestino/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                destino = JsonConvert.DeserializeObject<Destino>(apiResponse);
            }
            return View(await Task.Run(() => destino));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Destino reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync("updateDestino", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Destino destino = new Destino();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getDestino/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                destino = JsonConvert.DeserializeObject<Destino>(apiResponse);
            }
            return View(await Task.Run(() => destino));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.DeleteAsync($"deleteDestino/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return RedirectToAction("Index");
        }
    }
}
