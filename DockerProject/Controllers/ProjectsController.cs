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


    [NonAction]
    private void SetAccesRights()
    {
        ViewBag.UserCurent = _userManager.GetUserId(User);
        ViewBag.EsteAdmin = User.IsInRole("Admin");
    }

    [Authorize]
    public IActionResult Index(bool star = false, string? user = null, string? search = null)
    {
        SetAccesRights();

        IQueryable<Project> projects = _db.Projects
            .Include(p => p.Fields)
            .Include(p => p.Founder)
            .Include(p => p.StarredBy)
            .OrderByDescending(p => p.StarredBy.Count())
            .ThenBy(p => p.CreatedDate);

        if (star)
        {
            var currentUserId = _userManager.GetUserId(User);
            projects = projects.Where(p => p.StarredBy.Any(u => u.Id == currentUserId));
        }

        if (!string.IsNullOrEmpty(user))
        {
            projects = projects.Where(p => p.FounderId == user);
        }

        if (!string.IsNullOrEmpty(search))
        {
            projects = projects.Where(p => p.Title.Contains(search) || p.Description.Contains(search));
        }

        ViewBag.CurrentStar = star;
        ViewBag.CurrentUser = user;
        ViewBag.CurrentSearch = search;
        ViewBag.Projects = projects;

        if (TempData.ContainsKey("message"))
        {
            ViewBag.Message = TempData["message"];
            ViewBag.Alert = TempData["messageType"];
        }

        return View();
    }

    [Authorize]
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


    [Authorize]
    public IActionResult New()
    {
        Project project = new Project();

        return View(project);
    }

    [Authorize]
    [HttpPost]
    public IActionResult New(Project project)
    {
        project.FounderId = _userManager.GetUserId(User);
        project.CreatedDate = DateTime.Now;

        _db.Projects.Add(project);
        _db.SaveChanges();

        return RedirectToAction("Index");
    }

    [Authorize]
    public IActionResult Edit(string id)
    {
        Project? project = _db.Projects.Find(id);

        if (project is null)
            return NotFound();

        if (project.FounderId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
            return View(project);

        return RedirectToAction("Index");
    }

    [Authorize]
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


    [Authorize]
    [HttpPost]
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

    [Authorize]
    [HttpPost]
    public IActionResult Add2Favorites(string projectid)
    {
        var userId = _userManager.GetUserId(User);
        var user = _db.ApplicationUsers.Find(userId);
        var project = _db.Projects.Include(p => p.StarredBy).FirstOrDefault(p => p.Id == projectid);

        if (project is null)
            return NotFound();
        bool follow = true;
        if (project.StarredBy.Contains(user))
        {
            follow = false;
            project.StarredBy.Remove(user);
        }
        else
        {
            project.StarredBy.Add(user);
        }

        _db.SaveChanges();

        var nr_likes = project.StarredBy.Count();

        return Json(new { success = true, stars = nr_likes, follow = follow });
    }
}