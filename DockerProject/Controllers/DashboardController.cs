using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DockerProject.Controllers
{
    [Authorize] // ca sa fie logat ca sa poata accesa
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}