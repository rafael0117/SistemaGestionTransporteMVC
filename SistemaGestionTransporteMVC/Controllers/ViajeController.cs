using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
            List<Viaje> viajes = new List<Viaje>();
            List<Bus> buses = new List<Bus>();
            List<Destino> destinos = new List<Destino>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7252/api/");

                // Obtener viajes
                var responseViajes = await client.GetAsync("ViajeAPI/getViajes");
                var dataViajes = await responseViajes.Content.ReadAsStringAsync();
                viajes = JsonConvert.DeserializeObject<List<Viaje>>(dataViajes);

                // Obtener buses
                var responseBuses = await client.GetAsync("BusAPI/getBuses");
                var dataBuses = await responseBuses.Content.ReadAsStringAsync();
                buses = JsonConvert.DeserializeObject<List<Bus>>(dataBuses);

                // Obtener destinos
                var responseDestinos = await client.GetAsync("DestinoAPI/getDestinos");
                var dataDestinos = await responseDestinos.Content.ReadAsStringAsync();
                destinos = JsonConvert.DeserializeObject<List<Destino>>(dataDestinos);
            }

            // Asociar modelos a los viajes
            foreach (var viaje in viajes)
            {
                viaje.Bus = buses.FirstOrDefault(b => b.IdBus == viaje.IdBus);
                viaje.Destino = destinos.FirstOrDefault(d => d.IdDestino == viaje.IdDestino);
            }

            return View(viajes);
        }

        public async Task<IActionResult> Details(int id)
        {
            Viaje viaje = new Viaje();
            Bus bus = new Bus();
            Destino destino = new Destino();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7252/api/");

                // Obtener el viaje específico
                HttpResponseMessage responseViaje = await client.GetAsync($"ViajeAPI/getViaje/{id}");
                if (responseViaje.IsSuccessStatusCode)
                {
                    string apiResponseViaje = await responseViaje.Content.ReadAsStringAsync();
                    viaje = JsonConvert.DeserializeObject<Viaje>(apiResponseViaje);

                    // Obtener el bus relacionado
                    HttpResponseMessage responseBus = await client.GetAsync($"BusAPI/getBus/{viaje.IdBus}");
                    if (responseBus.IsSuccessStatusCode)
                    {
                        string apiResponseBus = await responseBus.Content.ReadAsStringAsync();
                        bus = JsonConvert.DeserializeObject<Bus>(apiResponseBus);
                    }

                    // Obtener el destino relacionado
                    HttpResponseMessage responseDestino = await client.GetAsync($"DestinoAPI/getDestino/{viaje.IdDestino}");
                    if (responseDestino.IsSuccessStatusCode)
                    {
                        string apiResponseDestino = await responseDestino.Content.ReadAsStringAsync();
                        destino = JsonConvert.DeserializeObject<Destino>(apiResponseDestino);
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            // Asignar los objetos bus y destino al modelo viaje
            viaje.Bus = bus;
            viaje.Destino = destino;

            return View(viaje);
        }

        public async Task<IActionResult> Create()
        {
            Viaje reg = new Viaje();

            // Cargar lista de buses
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7252/api/");
                HttpResponseMessage response = await client.GetAsync("BusAPI/getBuses");
                string apiResponse = await response.Content.ReadAsStringAsync();
                var buses = JsonConvert.DeserializeObject<List<Bus>>(apiResponse);

                ViewBag.Buses = buses.Select(b =>
                    new SelectListItem
                    {
                        Value = b.IdBus.ToString(),
                        Text = b.Modelo
                    }).ToList();
            }

            // Cargar lista de destinos
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7252/api/");
                HttpResponseMessage response = await client.GetAsync("DestinoAPI/getDestinos");
                string apiResponse = await response.Content.ReadAsStringAsync();
                var destinos = JsonConvert.DeserializeObject<List<Destino>>(apiResponse);

                ViewBag.Destinos = destinos.Select(d =>
                    new SelectListItem
                    {
                        Value = d.IdDestino.ToString(),
                        Text = d.nombre
                    }).ToList();
            }

            return View(reg);
        }
        [HttpPost]
        public async Task<IActionResult> Create(Viaje reg)
        {
            string mensaje = "";

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7252/api/ViajeAPI/");
                    StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync("insertViaje", content);

                    if (response.IsSuccessStatusCode)
                    {
                        mensaje = "Viaje registrado con éxito.";
                        return RedirectToAction("Index"); // o donde desees redirigir
                    }
                    else
                    {
                        mensaje = $"Error al registrar el viaje. Código de error: {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Ocurrió un error al intentar registrar el viaje: {ex.Message}";
            }

            // Si ocurre error, recargar listas
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7252/api/");
                HttpResponseMessage responseBuses = await client.GetAsync("BusAPI/getBuses");
                string dataBuses = await responseBuses.Content.ReadAsStringAsync();
                var buses = JsonConvert.DeserializeObject<List<Bus>>(dataBuses);

                ViewBag.Buses = buses.Select(b =>
                    new SelectListItem
                    {
                        Value = b.IdBus.ToString(),
                        Text = b.Modelo
                    }).ToList();

                HttpResponseMessage responseDestinos = await client.GetAsync("DestinoAPI/getDestinos");
                string dataDestinos = await responseDestinos.Content.ReadAsStringAsync();
                var destinos = JsonConvert.DeserializeObject<List<Destino>>(dataDestinos);

                ViewBag.Destinos = destinos.Select(d =>
                    new SelectListItem
                    {
                        Value = d.IdDestino.ToString(),
                        Text = d.nombre
                    }).ToList();
            }

            ViewBag.mensaje = mensaje;
            return View(reg);
        }



        public async Task<IActionResult> Edit(int id)
        {
            Viaje viaje = new Viaje();
            List<Bus> buses = new List<Bus>();
            List<Destino> destinos = new List<Destino>();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7252/api/");

                // Obtener el viaje específico
                HttpResponseMessage responseViaje = await client.GetAsync($"ViajeAPI/getViaje/{id}");
                if (responseViaje.IsSuccessStatusCode)
                {
                    string apiResponseViaje = await responseViaje.Content.ReadAsStringAsync();
                    viaje = JsonConvert.DeserializeObject<Viaje>(apiResponseViaje);
                }
                else
                {
                    return NotFound($"No se encontró el viaje con ID {id}.");
                }

                // Obtener la lista de buses
                HttpResponseMessage responseBuses = await client.GetAsync("BusAPI/getBuses");
                string apiResponseBuses = await responseBuses.Content.ReadAsStringAsync();
                buses = JsonConvert.DeserializeObject<List<Bus>>(apiResponseBuses);

                // Obtener la lista de destinos
                HttpResponseMessage responseDestinos = await client.GetAsync("DestinoAPI/getDestinos");
                string apiResponseDestinos = await responseDestinos.Content.ReadAsStringAsync();
                destinos = JsonConvert.DeserializeObject<List<Destino>>(apiResponseDestinos);
            }

            // Configurar ViewBag para ComboBoxes
            ViewBag.Buses = buses.Select(b => new SelectListItem
            {
                Value = b.IdBus.ToString(),
                Text = b.Modelo,
                Selected = b.IdBus == viaje.IdBus
            }).ToList();

            ViewBag.Destinos = destinos.Select(d => new SelectListItem
            {
                Value = d.IdDestino.ToString(),
                Text = d.nombre,
                Selected = d.IdDestino == viaje.IdDestino
            }).ToList();

            // Convertir fechas al formato adecuado
            viaje.FechaSalida = DateTime.Parse(viaje.FechaSalida.ToString("yyyy-MM-ddTHH:mm"));
            viaje.FechaLlegada = DateTime.Parse(viaje.FechaLlegada.ToString("yyyy-MM-ddTHH:mm"));

            return View(viaje);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Viaje reg)
        {
            string mensaje = "";
            List<Bus> buses = new List<Bus>();
            List<Destino> destinos = new List<Destino>();

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7252/api/ViajeAPI/");
                    StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PutAsync("updateViaje", content);

                    if (response.IsSuccessStatusCode)
                    {
                        mensaje = "Viaje actualizado con éxito.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        mensaje = $"Error al actualizar el viaje. Código de error: {response.StatusCode}";
                    }
                }
            }
            catch (Exception ex)
            {
                mensaje = $"Ocurrió un error: {ex.Message}";
            }

            // En caso de error, recargar las listas
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(apiUrl);

                // Recargar buses
                HttpResponseMessage responseBuses = await client.GetAsync("BusAPI/getBuses");
                string apiResponseBuses = await responseBuses.Content.ReadAsStringAsync();
                buses = JsonConvert.DeserializeObject<List<Bus>>(apiResponseBuses);

                // Recargar destinos
                HttpResponseMessage responseDestinos = await client.GetAsync("DestinoAPI/getDestinos");
                string apiResponseDestinos = await responseDestinos.Content.ReadAsStringAsync();
                destinos = JsonConvert.DeserializeObject<List<Destino>>(apiResponseDestinos);
            }

            ViewBag.Buses = buses.Select(b => new SelectListItem
            {
                Value = b.IdBus.ToString(),
                Text = b.Modelo
            }).ToList();

            ViewBag.Destinos = destinos.Select(d => new SelectListItem
            {
                Value = d.IdDestino.ToString(),
                Text = d.nombre
            }).ToList();

            ViewBag.mensaje = mensaje;
            return View(reg);
        }

        public async Task<IActionResult> Delete(int id)
        {
            Viaje viaje = new Viaje();
            Bus bus = new Bus();
            Destino destino = new Destino();

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7252/api/");

                // Obtener el viaje específico
                HttpResponseMessage responseViaje = await client.GetAsync($"ViajeAPI/getViaje/{id}");
                if (responseViaje.IsSuccessStatusCode)
                {
                    string apiResponseViaje = await responseViaje.Content.ReadAsStringAsync();
                    viaje = JsonConvert.DeserializeObject<Viaje>(apiResponseViaje);

                    // Obtener el bus relacionado
                    HttpResponseMessage responseBus = await client.GetAsync($"BusAPI/getBus/{viaje.IdBus}");
                    if (responseBus.IsSuccessStatusCode)
                    {
                        string apiResponseBus = await responseBus.Content.ReadAsStringAsync();
                        bus = JsonConvert.DeserializeObject<Bus>(apiResponseBus);
                    }

                    // Obtener el destino relacionado
                    HttpResponseMessage responseDestino = await client.GetAsync($"DestinoAPI/getDestino/{viaje.IdDestino}");
                    if (responseDestino.IsSuccessStatusCode)
                    {
                        string apiResponseDestino = await responseDestino.Content.ReadAsStringAsync();
                        destino = JsonConvert.DeserializeObject<Destino>(apiResponseDestino);
                    }
                }
                else
                {
                    return NotFound();
                }
            }

            // Asignar los objetos bus y destino al modelo viaje
            viaje.Bus = bus;
            viaje.Destino = destino;

            return View(viaje);
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