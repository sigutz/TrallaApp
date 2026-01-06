using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DockerProject.Data;
using DockerProject.Models;
using DockerProject.ViewModels;

namespace DockerProject.Controllers;

[Authorize]
public class DashboardController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public DashboardController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<IActionResult> Index(string? filter = null)
    {
        var userId = _userManager.GetUserId(User);

        var userProjects = await _context.ProjectMembers
            .Where(pm => pm.MemberId == userId)
            .Include(pm => pm.Project)
                .ThenInclude(p => p.Tasks)
            .Select(pm => pm.Project)
            .ToListAsync();

        var userTasksQuery = _context.Tasks
            .Where(t => t.Users.Any(u => u.Id == userId));

        userTasksQuery = filter switch
        {
            "done" => userTasksQuery.Where(t => t.Status == TaskStatusEnum.Done),
            "in-progress" => userTasksQuery.Where(t => t.Status == TaskStatusEnum.InProgress),
            "todo" => userTasksQuery.Where(t => t.Status == TaskStatusEnum.ToDo),
            "review" => userTasksQuery.Where(t => t.Status == TaskStatusEnum.Review),
            _ => userTasksQuery
        };

        var userTasks = await userTasksQuery.ToListAsync();

        var tasksByStatus = userTasks
            .GroupBy(t => t.Status)
            .ToDictionary(
                g => g.Key,
                g => g.Select(t => new TaskSummary
                {
                    Id = t.Id,
                    Title = t.Name,
                    ProjectName = t.ProjectParent.Title,
                    ProjectId = t.ProjectParent.Id,
                    Status = t.Status.ToString(),
                    Deadline = t.DeadLine
                }).ToList()
            );

        var upcomingDeadlines = userTasks
            .Where(t => t.DeadLine > DateTime.Now &&
                        t.DeadLine <= DateTime.Now.AddDays(7) &&
                        t.Status != TaskStatusEnum.Done)
            .OrderBy(t => t.DeadLine)
            .Select(t => new TaskSummary
            {
                Id = t.Id,
                Title = t.Name,
                ProjectName = t.ProjectParent.Title,
                ProjectId = t.ProjectParentId,
                Status = t.Status.ToString(),
                Deadline = t.DeadLine
            })
            .ToList();


        var viewModel = new DashboardViewModel
        {
            ActiveProjects = userProjects.Select(p => new ProjectSummary
            {
                Id = p.Id,
                Name = p.Title,
                TaskCount = p.Tasks.Count,
                CompletedTaskCount = p.Tasks.Count(t => t.Status == TaskStatusEnum.Done),
                NextDeadline = p.Tasks
                    .Where(t => t.DeadLine > DateTime.Now)
                    .OrderBy(t => t.DeadLine)
                    .FirstOrDefault()?.DeadLine
            }).ToList(),
            TasksByStatus = tasksByStatus
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.Value
                ),
            UpcomingDeadlines = upcomingDeadlines,
            CurrentFilter = filter
        };

        return View(viewModel);
    }
}
