using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaGestionTransporteMVC.Models;

namespace SistemaGestionTransporteMVC.Controllers
{
	public class VentaPasajeController : Controller
	{
		public async Task<IActionResult> Index()
		{
			List<VentaPasaje> temporal = new List<VentaPasaje>();
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("https://localhost:7102/api/VentaPasajeAPI/");
				HttpResponseMessage response = await client.GetAsync("getVentasPasaje");
				string apiResponse = await response.Content.ReadAsStringAsync();
				temporal = JsonConvert.DeserializeObject<List<VentaPasaje>>(apiResponse);
			}
			return View(await Task.Run(() => temporal));
		}

		public async Task<IActionResult> Create()
		{
			return View(await Task.Run(() => new VentaPasaje()));
		}

		[HttpPost]
		public async Task<IActionResult> Create(VentaPasaje reg)
		{
			string mensaje = "";
			using (var client = new HttpClient())
			{
				client.BaseAddress = new Uri("https://localhost:7102/api/VentaPasajeAPI/");
				StringContent content = new StringContent(JsonConvert.SerializeObject(reg),
														  Encoding.UTF8, "application/json");
				HttpResponseMessage response = await client.PostAsync("insertVentaPasaje", content);
				string apiResponse = await response.Content.ReadAsStringAsync();
				mensaje = apiResponse;
			}
			ViewBag.mensaje = mensaje;
			return View(await Task.Run(() => reg));
		}
	}
}
