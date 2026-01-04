using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using DockerProject.Models;

namespace DockerProject.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
         if (User.Identity is { IsAuthenticated: true })
             return RedirectToAction("Index", "Projects");
        // merge dar pana cred ca e cel mai ok pana la deployment ul final sa o lasam comentata ca sa putem sa testam
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
