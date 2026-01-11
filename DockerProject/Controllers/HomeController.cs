using System.Diagnostics;
using DockerProject.Data;
using Microsoft.AspNetCore.Mvc;
using DockerProject.Models;
using DockerProject.Services;
using Microsoft.AspNetCore.Identity;

namespace DockerProject.Controllers;

public class HomeController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : Controller
{
    private readonly ILogger<HomeController> _logger;

    private readonly ApplicationDbContext _db = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    public IActionResult Index()
    {
         if (User.Identity is { IsAuthenticated: true })
             return RedirectToAction("Index", "Projects");
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
