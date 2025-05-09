using System.Text;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SistemaGestionTransporteMVC.Models;

namespace SistemaGestionTransporteMVC.Controllers
{
	public class UsuarioController : Controller
	{
		private readonly string apiUrl = "https://localhost:7252/api/UsuarioAPI/";

		public async Task<IActionResult> Index()
		{
			List<Usuario> lista = new List<Usuario>();
			using (var client = new HttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(apiUrl + "getUsuarios");
				if (response.IsSuccessStatusCode)
				{
					string json = await response.Content.ReadAsStringAsync();
					lista = JsonConvert.DeserializeObject<List<Usuario>>(json);
				}
			}
			return View(lista);
		}

		public async Task<IActionResult> Details(int id)
		{
			Usuario usuario = new Usuario();
			using (var client = new HttpClient())
			{
				HttpResponseMessage response = await client.GetAsync(apiUrl + $"getUsuario/{id}");
				if (response.IsSuccessStatusCode)
				{
					string json = await response.Content.ReadAsStringAsync();
					usuario = JsonConvert.DeserializeObject<Usuario>(json);
				}
			}
			return View(usuario);
		}

		public IActionResult Create()
		{
			return View(new Usuario());
		}

		[HttpPost]
		public async Task<IActionResult> Create(Usuario reg)
		{
			using (var client = new HttpClient())
			{
				StringContent content = new StringContent(JsonConvert.SerializeObject(reg), Encoding.UTF8, "application/json");
				HttpResponseMessage response = await client.PostAsync(apiUrl + "insertUsuarioCliente", content);
				string result = await response.Content.ReadAsStringAsync();
				ViewBag.Mensaje = result;
			}
			return View(reg);
		}

        // Vista para el login
        public IActionResult Login()
        {
            return View(new Usuario());
        }

        // Acción para iniciar sesión
        [HttpPost]
        public async Task<IActionResult> Login(Usuario credenciales)
        {
            using (var client = new HttpClient())
            {
                var json = JsonConvert.SerializeObject(new
                {
                    idUsuario = 0,
                    nombres = "",
                    apellidos = "",
                    username = credenciales.Username,
                    clave = credenciales.Clave,
                    correo = "",
                    direccion = "",
                    idRol = 0
                });

                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl + "login", content);
                string jsonResponse = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    // Procesa la respuesta
                    dynamic resultado = JsonConvert.DeserializeObject<dynamic>(jsonResponse);
                    string rol = resultado.usuario.idRol.ToString();
                    int idUsuario = resultado.usuario.idUsuario;  // Obtener el idUsuario desde la respuesta

                    // Guardar la información del usuario en la sesión
                    HttpContext.Session.SetInt32("IdUsuario", idUsuario);  // Guardamos el IdUsuario
                    HttpContext.Session.SetString("Username", credenciales.Username);
                    HttpContext.Session.SetString("Rol", rol);

                    // Redirección según rol
                    switch (rol)
                    {
                        case "1": // Cliente
                            return RedirectToAction("Index", "DashBoard");

                        case "2": // Admin
                            return RedirectToAction("Index", "Cliente");

                        case "3": // Supervisor
                            return RedirectToAction("Panel", "Supervisor");

                        default:
                            ViewBag.Error = "Rol no reconocido.";
                            return View(credenciales);
                    }
                }
                else
                {
                    // Mostrar error si las credenciales son incorrectas
                    ViewBag.Error = "Credenciales inválidas.";
                    return View(credenciales);
                }
            }
        }

        public IActionResult Logout()
        {
            // Limpiar toda la sesión
            HttpContext.Session.Clear();

            // Redirigir al login o al inicio público
            return RedirectToAction("Login", "Usuario");
        }



    }
}
