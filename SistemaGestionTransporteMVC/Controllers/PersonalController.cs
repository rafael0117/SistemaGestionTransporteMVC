using Microsoft.AspNetCore.Mvc;

namespace SistemaGestionTransporteMVC.Controllers
{
    public class PersonalController : Controller
    {
        private readonly string apiUrl = "https://localhost:7252/api/RevisionBusAPI/";

        public IActionResult Index()
        {
            return View();
        }
    }
}
