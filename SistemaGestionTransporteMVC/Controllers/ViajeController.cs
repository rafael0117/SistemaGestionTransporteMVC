using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaGestionTransporteMVC.Models;
using System.Text;

namespace SistemaGestionTransporteMVC.Controllers
{
    public class ViajeController : Controller
    {
        private readonly string apiUrl = "https://localhost:7252/api/ViajeAPI/";

        public async Task<IActionResult> Index()
        {
            List<Viaje> temporal = new List<Viaje>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync("getViajes");
                string apiResponse = await response.Content.ReadAsStringAsync();
                temporal = JsonConvert.DeserializeObject<List<Viaje>>(apiResponse);
            }
            return View(await Task.Run(() => temporal));
        }

        public async Task<IActionResult> Details(int id)
        {
            Viaje viaje = new Viaje();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getViaje/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    viaje = JsonConvert.DeserializeObject<Viaje>(apiResponse);
                }
                else
                {
                    return NotFound();
                }
            }
            return View(await Task.Run(() => viaje));
        }

        public async Task<IActionResult> Create()
        {
            return View(await Task.Run(() => new Viaje()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Viaje reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("insertViaje", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }

        public async Task<IActionResult> Edit(int id)
        {
            Viaje viaje = new Viaje();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getViaje/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                viaje = JsonConvert.DeserializeObject<Viaje>(apiResponse);
            }
            return View(await Task.Run(() => viaje));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Viaje reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync("updateViaje", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Viaje viaje = new Viaje();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getViaje/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                viaje = JsonConvert.DeserializeObject<Viaje>(apiResponse);
            }
            return View(await Task.Run(() => viaje));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.DeleteAsync($"deleteViaje/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return RedirectToAction("Index");
        }
    }
}