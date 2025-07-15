using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Newtonsoft.Json;
using Rotativa.AspNetCore;
using Rotativa.AspNetCore.Options;
using SistemaGestionTransporteMVC.Models;
using SistemaGestionTransporteMVC.Utils;

namespace SistemaGestionTransporteMVC.Controllers
{
    public class CarritoController : Controller
    {
        private const string SessionKey = "Carrito";

        public IActionResult Index()
        {
            var carrito = HttpContext.Session.GetObjectFromJson<List<CarritoItem>>(SessionKey) ?? new List<CarritoItem>();
            ViewBag.Total = carrito.Sum(x => x.Subtotal);
            return View(carrito);
        }
        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] CarritoItem item)
        {
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
            {
                return Json(new { status = "not_logged_in" });
            }

            Viaje viaje = null;
            Destino destino = null;

            try
            {
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7252/api/");

                    // Obtener el viaje
                    var responseViaje = await client.GetAsync($"ViajeAPI/getViaje/{item.IdViaje}");
                    if (!responseViaje.IsSuccessStatusCode)
                    {
                        return Json(new { status = "error", message = "No se pudo obtener información del viaje." });
                    }

                    string apiResponse = await responseViaje.Content.ReadAsStringAsync();
                    viaje = JsonConvert.DeserializeObject<Viaje>(apiResponse);

                    if (viaje == null)
                    {
                        return Json(new { status = "error", message = "Viaje no encontrado." });
                    }

                    // Obtener el destino
                    var responseDestino = await client.GetAsync($"DestinoAPI/getDestino/{viaje.IdDestino}");
                    if (!responseDestino.IsSuccessStatusCode)
                    {
                        return Json(new { status = "error", message = "No se pudo obtener información del destino." });
                    }

                    string destinoResponse = await responseDestino.Content.ReadAsStringAsync();
                    destino = JsonConvert.DeserializeObject<Destino>(destinoResponse);

                    if (destino == null)
                    {
                        return Json(new { status = "error", message = "Destino no encontrado." });
                    }
                }

                // Obtener el carrito actual o inicializar uno nuevo
                var carrito = HttpContext.Session.GetObjectFromJson<List<CarritoItem>>(SessionKey) ?? new List<CarritoItem>();

                // Buscar si el viaje ya existe en el carrito
                var existente = carrito.FirstOrDefault(x => x.IdViaje == item.IdViaje);
                if (existente != null)
                {
                    existente.Cantidad += item.Cantidad;
                }
                else
                {
                    carrito.Add(new CarritoItem
                    {
                        IdViaje = viaje.IdViaje,
                        IdDestino = viaje.IdDestino,
                        nombreDestino = destino.nombre,
                        Precio = viaje.Precio,
                        Cantidad = item.Cantidad
                    });
                }

                // Guardar en sesión
                HttpContext.Session.SetObjectAsJson(SessionKey, carrito);

                return Json(new { status = "success", message = "El viaje se ha agregado al carrito." });
            }
            catch (Exception ex)
            {
                // Manejo de errores inesperados
                return Json(new { status = "error", message = "Ocurrió un error inesperado: " + ex.Message });
            }
        }



        public IActionResult Confirmacion()
        {
            ViewBag.Mensaje = TempData["Mensaje"];
            return View();
        }

        public IActionResult ResumenOrden()
        {
            // Verificar si el usuario está autenticado
            int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");
            if (idUsuario == null)
            {
                TempData["Error"] = "Debe iniciar sesión para ver el resumen de la compra.";
                return RedirectToAction("Login", "Usuario");
            }

            // Obtener el carrito de la sesión
            var carrito = HttpContext.Session.GetObjectFromJson<List<CarritoItem>>(SessionKey) ?? new List<CarritoItem>();

            // Calcular el total del carrito
            ViewBag.Total = carrito.Sum(x => x.Subtotal);

            return View(carrito);
        }

        [HttpPost]
        public async Task<IActionResult> GenerarVenta()
        {
            try
            {
                var carrito = HttpContext.Session.GetObjectFromJson<List<CarritoItem>>(SessionKey);

                if (carrito == null || !carrito.Any())
                {
                    TempData["Error"] = "El carrito está vacío.";
                    return RedirectToAction("ResumenOrden");
                }

                int? idUsuario = HttpContext.Session.GetInt32("IdUsuario");

                if (idUsuario == null)
                {
                    TempData["Error"] = "Debe iniciar sesión para realizar una compra.";
                    return RedirectToAction("Login", "Usuario");
                }
                var venta = new VentaPasaje
                {
                    Estado = "Registrado",
                    FechaVenta = DateTime.Now, // Formato de fecha ISO 8601
                    Total = carrito.Sum(c => c.Subtotal),
                    IdUsuario = idUsuario.Value,
                    Numero = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper(),
                    Detalles = carrito.Select(c => new DetalleVentaPasaje
                    {
                        IdViaje = c.IdViaje,
                        Cantidad = c.Cantidad,
                        Precio = c.Precio,
                        Total = c.Subtotal
                    }).ToList()
                };


                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:7252/api/VentaPasajeAPI/");
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.PostAsJsonAsync("registrar", venta);

                    if (response.IsSuccessStatusCode)
                    {
                        int idVenta = await response.Content.ReadFromJsonAsync<int>();

                        HttpContext.Session.Remove(SessionKey);

                        TempData["Mensaje"] = $"Venta generada con éxito. Código de venta: {idVenta}";
                        return RedirectToAction("ConfirmacionVenta", new { id = idVenta });
                    }
                    else
                    {
                        string errorDetalle = await response.Content.ReadAsStringAsync();

                        // Mostrar error en consola
                        Console.WriteLine("Error al generar venta:");
                        Console.WriteLine($"Status Code: {response.StatusCode}");
                        Console.WriteLine("Detalle del error:");
                        Console.WriteLine(errorDetalle);

                        TempData["Error"] = $"No se pudo completar la venta. Ver consola para más detalles.";
                        return RedirectToAction("ResumenOrden");
                    }
                }
            }
            catch (Exception ex)
            {
                // Mostrar error en consola
                Console.WriteLine("Excepción atrapada:");
                Console.WriteLine($"Mensaje: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"InnerException: {ex.InnerException.Message}");
                }

                TempData["Error"] = $"Error inesperado. Ver consola para más detalles.";
                return RedirectToAction("ResumenOrden");
            }
        }
        public async Task<IActionResult> ConfirmacionVenta(int id)
        {
            VentaPasaje venta = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7252/api/");
                HttpResponseMessage response = await client.GetAsync($"VentaPasajeAPI/obtenerporid/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    venta = JsonConvert.DeserializeObject<VentaPasaje>(apiResponse);
                }
                else
                {
                    TempData["Error"] = "No se pudo obtener la información de la venta.";
                    return RedirectToAction("Index", "Cliente");
                }
            }

            return View(venta); // Pasa los datos de la venta a la vista
        }

        public async Task<IActionResult> GenerarPDF(int id)
        {
            VentaPasaje venta = null;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("https://localhost:7252/api/");
                HttpResponseMessage response = await client.GetAsync($"VentaPasajeAPI/obtenerporid/{id}");

                if (response.IsSuccessStatusCode)
                {
                    string apiResponse = await response.Content.ReadAsStringAsync();
                    venta = JsonConvert.DeserializeObject<VentaPasaje>(apiResponse);
                }
                else
                {
                    return NotFound("No se pudo generar el PDF: venta no encontrada.");
                }
            }

            DateTime hoy = DateTime.Now;

            return new ViewAsPdf("GenerarPDF", venta)
            {
                FileName = $"factura-{hoy:yyyyMMddHHmmss}.pdf",
                PageOrientation = Orientation.Portrait,
                PageSize = Size.A4,
                CustomSwitches = "--disable-smart-shrinking"
            };
        }







    }


}

