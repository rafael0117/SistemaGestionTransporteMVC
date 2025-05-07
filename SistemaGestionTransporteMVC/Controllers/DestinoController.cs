using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaGestionTransporteMVC.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using static System.Net.WebRequestMethods;

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

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Destino destino)
        {
            // Validamos si se ha seleccionado una imagen
            if (destino.ImagenUrl == null || destino.ImagenUrl.Length == 0)
            {
                ModelState.AddModelError("ImagenUrl", "Debe seleccionar una imagen.");
                return View(destino);
            }

            // Procedemos con la carga de la imagen
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                using (var form = new MultipartFormDataContent())
                {
                    // Agregamos otros campos al formulario
                    form.Add(new StringContent(destino.nombre), "nombre");

                    // Agregamos el archivo de la imagen
                    var fileContent = new StreamContent(destino.ImagenUrl.OpenReadStream());
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(destino.ImagenUrl.ContentType);
                    form.Add(fileContent, "imagen", destino.ImagenUrl.FileName);

                    // Enviamos la solicitud HTTP
                    HttpResponseMessage response = await client.PostAsync("insertDestino", form);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("Index");
                    }

                    // En caso de error, mostramos el mensaje
                    ViewBag.Error = await response.Content.ReadAsStringAsync();
                    return View(destino);
                }
            }
        }



        [HttpGet]
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
            return View(destino);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Destino reg, IFormFile ImagenUrl)
        {
            // Validamos si se ha seleccionado una imagen
            if (ImagenUrl != null && ImagenUrl.Length > 0)
            {
                string extension = Path.GetExtension(ImagenUrl.FileName).ToLowerInvariant();
                string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };

                if (!allowedExtensions.Contains(extension))
                {
                    ModelState.AddModelError("ImagenUrl", "El archivo debe ser una imagen válida");
                    return View(reg);
                }

                if (ImagenUrl.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("ImagenUrl", "La imagen no puede exceder 5MB");
                    return View(reg);
                }
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);
                using (var form = new MultipartFormDataContent())
                {
                    form.Add(new StringContent(reg.nombre), "nombre");

                    if (ImagenUrl != null && ImagenUrl.Length > 0)
                    {
                        var fileContent = new StreamContent(ImagenUrl.OpenReadStream());
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue(ImagenUrl.ContentType);
                        form.Add(fileContent, "imagen", ImagenUrl.FileName);
                    }

                    HttpResponseMessage response = await client.PutAsync($"updateDestino/{reg.IdDestino}", form);
                    if (response.IsSuccessStatusCode)
                        return RedirectToAction("Index");

                    ViewBag.Error = await response.Content.ReadAsStringAsync();
                    return View(reg);
                }
            }
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
