using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaGestionTransporteMVC.Models;
using System.Text;

namespace SistemaGestionTransporteMVC.Controllers
{
    public class BusController : Controller
    {
        private readonly string apiUrl = "https://localhost:7252/api/BusAPI/";

        public async Task<IActionResult> Index()
        {
            List<Bus> temporal = new List<Bus>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync("getBuses");
                string apiResponse = await response.Content.ReadAsStringAsync();
                temporal = JsonConvert.DeserializeObject<List<Bus>>(apiResponse);
            }
            return View(await Task.Run(() => temporal));
        }

        public async Task<IActionResult> Details(int id)
        {
            Bus bus = new Bus();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getBus/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    bus = JsonConvert.DeserializeObject<Bus>(apiResponse);
                }
                else
                {
                    return NotFound();
                }
            }
            return View(await Task.Run(() => bus));
        }
        public async Task<IActionResult> Create()
        {
            return View(await Task.Run(() => new Bus()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(Bus reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("insertBus", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }

        public async Task<IActionResult> Edit(int id)
        {
            Bus bus = new Bus();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getBus/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                bus = JsonConvert.DeserializeObject<Bus>(apiResponse);
            }
            return View(await Task.Run(() => bus));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Bus reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync("updateBus", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }

        public async Task<IActionResult> Delete(int id)
        {
            Bus bus = new Bus();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getBus/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                bus = JsonConvert.DeserializeObject<Bus>(apiResponse);
            }
            return View(await Task.Run(() => bus));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.DeleteAsync($"deleteBus/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return RedirectToAction("Index");
        }
    }
}
