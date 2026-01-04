using DockerProject.Data;
using DockerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DockerProject.Controllers;

public class ProjectTasksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ProjectTasksController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // POST: ProjectTasks/Create - Creează task prin AJAX
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(string projectId, string name, string description, DateTime? deadline)
    {
        var project = await _context.Projects.FindAsync(projectId);
        
        if (project == null) return NotFound();
        
        // Verifică dacă user-ul e fondatorul proiectului
        if (project.FounderId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            return Forbid();
        
        var task = new ProjectTask
        {
            Name = name,
            Description = description ?? string.Empty,
            DeadLine = deadline ?? DateTime.Now.AddDays(7),
            ProjectParentId = projectId,
            AssignedDate = DateTime.Now
        };
        
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        
        return Json(new { success = true, taskId = task.Id, taskName = task.Name });
    }

    // POST: ProjectTasks/Edit - Editează task prin AJAX
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit(string id, string name, string description, DateTime? deadline, TaskStatusEnum? status)
    {
        var task = await _context.Tasks
            .Include(t => t.ProjectParent)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (task == null) return NotFound();
        
        // Verifică dacă user-ul e fondatorul proiectului
        if (task.ProjectParent.FounderId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            return Forbid();

        task.Name = name;
        task.Description = description ?? task.Description;
        task.DeadLine = deadline ?? task.DeadLine;
        
        if (status.HasValue)
        {
            task.Status = status.Value;
            if (status.Value == TaskStatusEnum.Done && task.DoneDate == default)
            {
                task.DoneDate = DateTime.Now;
            }
        }

        await _context.SaveChangesAsync();
        return Json(new { success = true });
    }

    // POST: ProjectTasks/Delete - Șterge task prin AJAX
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var task = await _context.Tasks
            .Include(t => t.ProjectParent)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (task == null) return NotFound();
        
        // Verifică dacă user-ul e fondatorul proiectului
        if (task.ProjectParent.FounderId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            return Forbid();
        
        _context.Tasks.Remove(task);
        await _context.SaveChangesAsync();
        
        return Json(new { success = true });
    }

    // POST: ProjectTasks/UpdateStatus - Schimbă statusul prin AJAX
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(string id, TaskStatusEnum status)
    {
        var task = await _context.Tasks
            .Include(t => t.ProjectParent)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (task == null) return NotFound();
        
        // Verifică dacă user-ul e fondatorul proiectului
        if (task.ProjectParent.FounderId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            return Forbid();

        task.Status = status;
        if (status == TaskStatusEnum.Done)
            task.DoneDate = DateTime.Now;

        await _context.SaveChangesAsync();
        return Json(new { success = true, status = status.ToString() });
    }
}