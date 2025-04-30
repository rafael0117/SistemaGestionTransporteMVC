using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaGestionTransporteMVC.Models;
using System.Text;

namespace SistemaGestionTransporteMVC.Controllers
{
    public class RevisionBusController : Controller
    {
        private readonly string apiUrl = "https://localhost:7252/api/RevisionBusAPI/";

        public async Task<IActionResult> Index()
        {
            List<RevisionBus> temporal = new List<RevisionBus>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync("getRevisionBuses");
                string apiResponse = await response.Content.ReadAsStringAsync();
                temporal = JsonConvert.DeserializeObject<List<RevisionBus>>(apiResponse);
            }
            return View(await Task.Run(() => temporal));
        }

        public async Task<IActionResult> Details(int id)
        {
            RevisionBus revision = new RevisionBus();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getRevisionBus/{id}");
                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    revision = JsonConvert.DeserializeObject<RevisionBus>(apiResponse);
                }
                else
                {
                    return NotFound();
                }
            }
            return View(await Task.Run(() => revision));
        }

        public async Task<IActionResult> Create()
        {
            return View(await Task.Run(() => new RevisionBus()));
        }

        [HttpPost]
        public async Task<IActionResult> Create(RevisionBus reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync("insertRevisionBus", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }

        public async Task<IActionResult> Edit(int id)
        {
            RevisionBus revision = new RevisionBus();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getRevisionBus/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                revision = JsonConvert.DeserializeObject<RevisionBus>(apiResponse);
            }
            return View(await Task.Run(() => revision));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RevisionBus reg)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync("updateRevisionBus", content);
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return View(await Task.Run(() => reg));
        }

        public async Task<IActionResult> Delete(int id)
        {
            RevisionBus revision = new RevisionBus();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.GetAsync($"getRevisionBus/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                revision = JsonConvert.DeserializeObject<RevisionBus>(apiResponse);
            }
            return View(await Task.Run(() => revision));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            string mensaje = "";
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                HttpResponseMessage response = await client.DeleteAsync($"deleteRevisionBus/{id}");
                string apiResponse = await response.Content.ReadAsStringAsync();
                mensaje = apiResponse;
            }
            ViewBag.mensaje = mensaje;
            return RedirectToAction("Index");
        }
    }
}
