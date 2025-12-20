using Microsoft.AspNetCore.Mvc;

namespace DockerProject.Controllers;

public class FieldsController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
    
}