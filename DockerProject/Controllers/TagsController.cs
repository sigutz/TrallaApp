using DockerProject.Data;
using DockerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DockerProject.Controllers;

public class TagsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;

    public TagsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _db = context;
        _userManager = userManager;
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(string name, string hexColor, string taskId)
    {
        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(taskId))
            return BadRequest("Invalid data");

        var task = await _db.Tasks.Include(t => t.Tags).FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null) return NotFound();

        // 1. Create the new Tag
        var newTag = new Tag
        {
            Name = name,
            HexColor = hexColor ?? "#000000"
        };

        _db.Tags.Add(newTag);
        
        // 2. Assign it to the task immediately
        task.Tags.Add(newTag);
        
        await _db.SaveChangesAsync();

        return Json(new { success = true, id = newTag.Id, name = newTag.Name, color = newTag.HexColor });
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Assign(string tagId, string taskId)
    {
        var task = await _db.Tasks.Include(t => t.Tags).FirstOrDefaultAsync(t => t.Id == taskId);
        var tag = await _db.Tags.FindAsync(tagId);

        if (task == null || tag == null) return NotFound();

        // Check if already assigned to avoid duplicates
        if (!task.Tags.Any(t => t.Id == tagId))
        {
            task.Tags.Add(tag);
            await _db.SaveChangesAsync();
        }

        return Json(new { success = true, id = tag.Id, name = tag.Name, color = tag.HexColor });
    }
    
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Remove(string tagId, string taskId)
    {
        var task = await _db.Tasks.Include(t => t.Tags).FirstOrDefaultAsync(t => t.Id == taskId);
        
        if (task == null) return NotFound();
        
        var tagToRemove = task.Tags.FirstOrDefault(t => t.Id == tagId);
        if (tagToRemove != null)
        {
            task.Tags.Remove(tagToRemove);
            await _db.SaveChangesAsync();
        }

        return Json(new { success = true });
    }
}