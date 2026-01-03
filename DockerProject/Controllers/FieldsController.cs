using DockerProject.Data;
using DockerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DockerProject.Controllers;

public class FieldsController(
    ApplicationDbContext context,
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : Controller
{
    private readonly ApplicationDbContext _db = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;

    [HttpPost]
    [Authorize]
    public IActionResult New(Field reqProject)
    {
        _db.Fields.Add(reqProject);
        _db.SaveChanges();

        return Json(new { 
            success = true, 
            id = reqProject.Id, 
            title = reqProject.Title, 
            color = reqProject.HexColor 
        });    }

    [HttpPost]
    [Authorize]
    public IActionResult AddField2Project(string fieldId, string projectId)
    {
        
        var field = _db.Fields.Find(fieldId);
        
        var project = _db.Projects.Find(projectId);
        if (project is null)
            return NotFound();
        field.Projects.Add(project);
        _db.SaveChanges();
        return Json(new { success = true });
    }
}