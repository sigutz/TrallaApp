using DockerProject.Data;
using DockerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DockerProject.ViewModels;
namespace DockerProject.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _db = context;
        _userManager = userManager;
    }

    // GET: Admin/Index - Dashboard principal
    public async Task<IActionResult> Index()
    {
        var viewModel = new AdminDashboardViewModel
        {
            TotalUsers = await _userManager.Users.CountAsync(),
            TotalProjects = await _db.Projects.CountAsync(),
            TotalTasks = await _db.Tasks.CountAsync(),
            TotalComments = await _db.Comments.CountAsync(),

            RecentProjects = await _db.Projects
                .Include(p => p.Founder)
                .OrderByDescending(p => p.CreatedDate)
                .Take(5)
                .ToListAsync(),

            RecentUsers = await _userManager.Users
                .OrderByDescending(u => u.Id) // Assuming newer users have higher IDs
                .Take(5)
                .ToListAsync(),

            RecentComments = await _db.Comments
                .Include(c => c.Author)
                .Include(c => c.ProjectParent)
                .OrderByDescending(c => c.Date)
                .Take(5)
                .ToListAsync(),

            // Statistics
            PendingMemberships = await _db.ProjectMembers
                .CountAsync(pm => pm.Status == ProjectMemberStatus.Pending),

            CompletedTasks = await _db.Tasks
                .CountAsync(t => t.Status == TaskStatusEnum.Done),

            InProgressTasks = await _db.Tasks
                .CountAsync(t => t.Status == TaskStatusEnum.InProgress)
        };

        return View(viewModel);
    }

    // GET: Admin/AllProjects - Listă completă proiecte
    public async Task<IActionResult> AllProjects(string? search = null, int page = 1)
    {
        var projectsQuery = _db.Projects
            .Include(p => p.Founder)
            .Include(p => p.Tasks)
            .AsQueryable();

        if (!string.IsNullOrEmpty(search))
        {
            projectsQuery = projectsQuery.Where(p =>
                p.Title.Contains(search) ||
                p.Description.Contains(search));
        }

        int perPage = 10;
        int totalProjects = await projectsQuery.CountAsync();
        int totalPages = (int)Math.Ceiling(totalProjects / (double)perPage);

        var projects = await projectsQuery
            .OrderByDescending(p => p.CreatedDate)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        ViewBag.Search = search;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(projects);
    }

    // GET: Admin/AllTasks - Listă completă task-uri
    public async Task<IActionResult> AllTasks(string? status = null, int page = 1)
    {
        var tasksQuery = _db.Tasks
            .Include(t => t.ProjectParent)
            .Include(t => t.Users)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<TaskStatusEnum>(status, out var taskStatus))
        {
            tasksQuery = tasksQuery.Where(t => t.Status == taskStatus);
        }

        int perPage = 15;
        int totalTasks = await tasksQuery.CountAsync();
        int totalPages = (int)Math.Ceiling(totalTasks / (double)perPage);

        var tasks = await tasksQuery
            .OrderByDescending(t => t.AssignedDate)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        ViewBag.Status = status;
        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(tasks);
    }

    // GET: Admin/AllComments - Listă completă comentarii
    public async Task<IActionResult> AllComments(int page = 1)
    {
        int perPage = 20;

        var totalComments = await _db.Comments.CountAsync();
        int totalPages = (int)Math.Ceiling(totalComments / (double)perPage);

        var comments = await _db.Comments
            .Include(c => c.Author)
            .Include(c => c.ProjectParent)
            .Include(c => c.TaskParent)
            .OrderByDescending(c => c.Date)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync();

        ViewBag.CurrentPage = page;
        ViewBag.TotalPages = totalPages;

        return View(comments);
    }

    // POST: Admin/DeleteComment/{id}
    [HttpPost]
    public async Task<IActionResult> DeleteComment(string id)
    {
        var comment = await _db.Comments.FindAsync(id);
        if (comment == null)
            return Json(new { success = false, message = "Comentariul nu a fost găsit." });

        _db.Comments.Remove(comment);
        await _db.SaveChangesAsync();

        return Json(new { success = true, message = "Comentariu șters cu succes!" });
    }

}

