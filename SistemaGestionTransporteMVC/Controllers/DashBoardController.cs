using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaGestionTransporteMVC.Models;

namespace SistemaGestionTransporteMVC.Controllers
{
    public class DashBoardController : Controller
    {
        private readonly string apiUrl = "https://localhost:7252/api/DashboardAPI/resumen";

        public async Task<IActionResult> Index()
        {
            DashboardResumen resumen = new DashboardResumen();

            using (var client = new HttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(apiUrl);
                if (response.IsSuccessStatusCode)
                {
                    string jsonData = await response.Content.ReadAsStringAsync();
                    resumen = JsonConvert.DeserializeObject<DashboardResumen>(jsonData);
                }
            }

            return View(resumen);
        }
    }
}
