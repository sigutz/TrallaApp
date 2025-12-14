using DockerProject.Data;
using DockerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DockerProject.Controllers;

public class ProjectsController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : Controller
{
    private readonly ApplicationDbContext _db = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    [Authorize(Roles = "User,Editor,Admin")]
    public IActionResult Index()
    {
        var projects = _db.Projects
            .Include(p => p.Fields)
            .Include(p => p.Founder)
            .Include(p => p.StarredBy);
        ViewBag.Projects = projects;
        return View();
    }

    [Authorize(Roles = "User,Editor,Admin")]
    public IActionResult Show(string id)
    {
        Project? project = _db.Projects
            .Include(p => p.Founder)
            .Include(p => p.Fields)
            .Include(p => p.StarredBy)
            .Include(p => p.Tasks)
            .Include(p => p.Comments)
                .ThenInclude(c => c.Author)
            .Where(p => p.Id == id).FirstOrDefault();

        if (project is null)
            return NotFound();
        
        return View(project);
    }


    [Authorize(Roles = "User,Editor,Admin")]
    public IActionResult New()
    {
        Project project = new Project();
        return View(project);
    }
    
    [Authorize(Roles = "User,Editor,Admin")]
    [HttpPost]
    public IActionResult New(Project project)
    {
        return RedirectToAction("Index");
    }

    [Authorize(Roles = "User,Editor,Admin")]
    [HttpPost]
    public IActionResult Edit(string id)
    {
        Project? project = _db.Projects.Find(id);

        if (project is null)
            return NotFound();

        if (project.FounderId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            return View(project);

        return RedirectToAction("Index");
    }
}