using Microsoft.AspNetCore.Mvc;

namespace SistemaGestionTransporteMVC.Controllers
{
    public class CarritoController : Controller
    {
        public IActionResult VerCarrito()
        {
           return View();
        }

    }
}
