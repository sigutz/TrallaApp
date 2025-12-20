using DockerProject.Data;
using DockerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

    private void SetAccesRights()
    {
        ViewBag.ListaProiecteProprii = from p in _db.Projects
            where p.FounderId == _userManager.GetUserId(User)
            select p.Id;

        ViewBag.UserCurent = _userManager.GetUserId(User);
        ViewBag.EsteAdmin = User.IsInRole("Admin");
    }

    [NonAction]
    public ICollection<Field> GetAllFields()
    {
        var selectList = new List<Field>();

        var fields = _db.Fields;

        foreach (var field in fields)
        {
            selectList.Add(field);
        }

        return selectList;
    }

    [Authorize(Roles = "User,Editor,Admin")]
    public IActionResult Index()
    {
        SetAccesRights();

        var projects = _db.Projects
            .Include(p => p.Fields)
            .Include(p => p.Founder)
            .Include(p => p.StarredBy)
            .OrderByDescending(p => p.StarredBy.Count())
            .ThenBy(p => p.CreatedDate);

        ViewBag.Projects = projects;

        if (TempData.ContainsKey("message"))
        {
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
        }

        return View();
    }

    [Authorize(Roles = "User,Editor,Admin")]
    public IActionResult Profile(string userid)
    {
        SetAccesRights();

        if (_db.ApplicationUsers.Find(userid) is null)
            return NotFound();

        var projects = _db.Projects
            .Include(p => p.Fields)
            .Include(p => p.Founder)
            .Include(p => p.StarredBy)
            .OrderByDescending(p => p.StarredBy.Count())
            .ThenBy(p => p.CreatedDate)
            .Where(p => p.FounderId == userid);

        ViewBag.Projects = projects;

        if (TempData.ContainsKey("message"))
        {
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
        }

        return View("Index");
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
            .Include(p => p.Comments)
            .ThenInclude(c => c.Votes)
            .FirstOrDefault(p => p.Id == id);

        if (project is null)
            return NotFound();

        return View(project);
    }


    [Authorize(Roles = "User,Editor,Admin")]
    public IActionResult New()
    {
        Project project = new Project();

        project.Fields = GetAllFields();

        return View(project);
    }

    [Authorize(Roles = "User,Editor,Admin")]
    [HttpPost]
    public IActionResult New(Project project)
    {
        project.FounderId = _userManager.GetUserId(User);
        project.CreatedDate = DateTime.Now;

        _db.Projects.Add(project);
        _db.SaveChanges();

        return RedirectToAction("Index");
    }

    [Authorize(Roles = "User,Editor,Admin")]
    public IActionResult Edit(string id)
    {
        Project? project = _db.Projects.Find(id);

        if (project is null)
            return NotFound();

        if (project.FounderId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            return View(project);

        return RedirectToAction("Index");
    }

    [Authorize(Roles = "User,Editor,Admin")]
    [HttpPost]
    public IActionResult Edit(string id, Project reqProject)
    {
        Project? project = _db.Projects.Find(id);

        if (project is null)
            return NotFound();

        if (project.FounderId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
        {
            try
            {
                project.Title = reqProject.Title;
                project.Description = reqProject.Description;
                project.Fields = reqProject.Fields;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception)
            {
                ViewBag.Project = project;
                return View(project);
            }
        }

        return Forbid();
    }

    [HttpPost]
    [Authorize(Roles = "User,Editor,Admin")]
    public ActionResult Delete(string projectid)
    {
        Project? project = _db.Projects.Find(projectid);

        if (project is null)
            return NotFound();
        if (_userManager.GetUserId(User) != project.FounderId && !User.IsInRole("Admin"))
            return Forbid();

        _db.Projects.Remove(project);
        _db.SaveChanges();
        return RedirectToAction("Index");
    }
}