using DockerProject.Data;
using DockerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace DockerProject.Controllers;

public class ProjectTasksController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICompositeViewEngine _viewEngine;
    private readonly ITempDataProvider _tempDataProvider;

    public ProjectTasksController(
        ApplicationDbContext context, 
        UserManager<ApplicationUser> userManager,
        ICompositeViewEngine viewEngine,
        ITempDataProvider tempDataProvider)
    {
        _db = context;
        _userManager = userManager;
        _viewEngine = viewEngine;
        _tempDataProvider = tempDataProvider;
    }

    // POST: ProjectTasks/Create - Creates blank task & returns HTML
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create(string projectId, TaskStatusEnum status)
    {
        var project = await _db.Projects.FindAsync(projectId);
        
        if (project == null) return NotFound();
        
        // Check permissions
        if (project.FounderId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            return Forbid();
        
        // Create Blank Task
        var task = new ProjectTask
        {
            Name = "New Task", // Default name
            Description = string.Empty,
            DeadLine = DateTime.Now.AddDays(7),
            ProjectParentId = projectId,
            AssignedDate = DateTime.Now,
            Status = status // Set the status based on which button was clicked
        };
        
        _db.Tasks.Add(task);
        await _db.SaveChangesAsync();

        // Reload task to ensure relationships (like Tags/Comments) are null-safe for the View
        // Although a new task has none, the partial might check for them.
        var taskForView = await _db.Tasks
            .Include(t => t.Tags)
            .Include(t => t.Comments)
            .Include(t => t.Users)
            .FirstOrDefaultAsync(t => t.Id == task.Id);
        
        // Render the _ShowTask partial to a string
        string htmlString = await RenderViewAsync("~/Views/Shared/_ShowTask.cshtml", taskForView, true);
        
        return Json(new { success = true, html = htmlString });
    }

    // POST: ProjectTasks/Edit
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Edit(string id, string name, string description, DateTime? deadline, TaskStatusEnum? status)
    {
        var task = await _db.Tasks
            .Include(t => t.ProjectParent)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (task == null) return NotFound();
        
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

        await _db.SaveChangesAsync();
        return Json(new { success = true });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditTitle(string taskId, string newTitle)
    {
        var task = await _db.Tasks
            .Include(t => t.ProjectParent)
            .FirstOrDefaultAsync(t => t.Id == taskId);
        
        if (task == null) return NotFound();
        
        if (task.ProjectParent.FounderId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            return Forbid();
        
        task.Name = newTitle;

        await _db.SaveChangesAsync();
        return Json(new { success = true });
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditDescription(string taskId, string description)
    {
        var task = await _db.Tasks
            .Include(t => t.ProjectParent)
            .FirstOrDefaultAsync(t => t.Id == taskId);
        
        if (task == null) return NotFound();
        
        if (task.ProjectParent.FounderId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            return Forbid();
        
        task.Description = description;

        await _db.SaveChangesAsync();
        return Json(new { success = true });
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditDeadLine(string taskId, DateTime deadline)
    {
        var task = await _db.Tasks
            .Include(t => t.ProjectParent)
            .FirstOrDefaultAsync(t => t.Id == taskId);
        
        if (task == null) return NotFound();
        
        if (task.ProjectParent.FounderId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            return Forbid();
        
        task.DeadLine = deadline;

        await _db.SaveChangesAsync();
        return Json(new { success = true });
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> EditStatus(string taskId, TaskStatusEnum statusEnum)
    {
        var task = await _db.Tasks
            .Include(t => t.ProjectParent)
            .FirstOrDefaultAsync(t => t.Id == taskId);
        
        if (task == null) return NotFound();
        
        if (task.ProjectParent.FounderId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            return Forbid();
        
        task.Status = statusEnum;

        await _db.SaveChangesAsync();
        return Json(new { success = true });
    }
    
    // POST: ProjectTasks/Delete
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var task = await _db.Tasks
            .Include(t => t.ProjectParent)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (task == null) return NotFound();
        
        if (task.ProjectParent.FounderId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            return Forbid();
        
        _db.Tasks.Remove(task);
        await _db.SaveChangesAsync();
        
        return Json(new { success = true });
    }

    // POST: ProjectTasks/UpdateStatus
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(string id, TaskStatusEnum status)
    {
        var task = await _db.Tasks
            .Include(t => t.ProjectParent)
            .FirstOrDefaultAsync(t => t.Id == id);
        
        if (task == null) return NotFound();
        
        if (task.ProjectParent.FounderId != _userManager.GetUserId(User) && !User.IsInRole("Admin"))
            return Forbid();

        task.Status = status;
        if (status == TaskStatusEnum.Done)
            task.DoneDate = DateTime.Now;

        await _db.SaveChangesAsync();
        return Json(new { success = true, status = status.ToString() });
    }

    // Helper to render View to String
    private async Task<string> RenderViewAsync<TModel>(string viewName, TModel model, bool partial = false)
    {
        if (string.IsNullOrEmpty(viewName))
            viewName = ControllerContext.ActionDescriptor.ActionName;

        ViewData.Model = model;

        using (var writer = new StringWriter())
        {
            IViewEngine viewEngine = _viewEngine;
            ViewEngineResult viewResult = viewEngine.FindView(ControllerContext, viewName, !partial);

            if (!viewResult.Success)
                viewResult = viewEngine.GetView(null, viewName, !partial);

            if (!viewResult.Success)
                return $"A view with the name {viewName} could not be found";

            ViewContext viewContext = new ViewContext(
                ControllerContext,
                viewResult.View,
                ViewData,
                TempData,
                writer,
                new HtmlHelperOptions()
            );

            await viewResult.View.RenderAsync(viewContext);
            return writer.GetStringBuilder().ToString();
        }
    }
}