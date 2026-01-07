using DockerProject.Data;
using DockerProject.Models;
using DockerProject.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DockerProject.Controllers;

[Authorize]
public class UsersController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UsersController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _db = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: Users/Index - Lista tuturor utilizatorilor
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var users = _userManager.Users.ToList();
        var userViewModels = new List<UserViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            userViewModels.Add(new UserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                LockoutEnd = user.LockoutEnd,
                IsActive = user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.Now,
                Roles = roles.ToList()
            });
        }

        return View(userViewModels);
    }

    // POST: Users/ToggleLockout - Activează/Dezactivează utilizator
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ToggleLockout(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        // Previne auto-lockout
        if (user.Id == _userManager.GetUserId(User))
            return Json(new { success = false, message = "Nu te poți dezactiva pe tine însuți!" });

        if (user.LockoutEnd == null || user.LockoutEnd <= DateTimeOffset.Now)
        {
            // Dezactivează - lockout permanent
            await _userManager.SetLockoutEndDateAsync(user, DateTimeOffset.MaxValue);
            return Json(new { success = true, isActive = false });
        }
        else
        {
            // Activează - elimină lockout
            await _userManager.SetLockoutEndDateAsync(user, null);
            return Json(new { success = true, isActive = true });
        }
    }

    // POST: Users/ChangeRole - Schimbă rolul utilizatorului
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> ChangeRole(string userId, string newRole)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        // Previne auto-demotion
        if (user.Id == _userManager.GetUserId(User))
            return Json(new { success = false, message = "Nu îți poți schimba propriul rol!" });

        // Verifică dacă rolul există
        if (!await _roleManager.RoleExistsAsync(newRole))
            return Json(new { success = false, message = "Rol invalid!" });

        // Elimină toate rolurile existente
        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Adaugă noul rol
        await _userManager.AddToRoleAsync(user, newRole);

        return Json(new { success = true, role = newRole });
    }

    // POST: Users/Delete - Șterge utilizator
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Delete(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return NotFound();

        // Previne auto-delete
        if (user.Id == _userManager.GetUserId(User))
            return Json(new { success = false, message = "Nu te poți șterge pe tine însuți!" });

        var result = await _userManager.DeleteAsync(user);
        
        if (result.Succeeded)
            return Json(new { success = true });
        
        return Json(new { success = false, message = "Eroare la ștergerea utilizatorului" });
    }
    
[Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();
    
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();
    
        var roles = await _userManager.GetRolesAsync(user);
        var allRoles = _roleManager.Roles.Select(r => r.Name).ToList();
    
        var viewModel = new EditUserViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            CurrentRoles = roles.ToList(),
            AllRoles = allRoles
        };
    
        return View(viewModel);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> Edit(EditUserViewModel model)
    {
        var user = await _userManager.FindByIdAsync(model.Id);
        if (user == null)
            return NotFound();
    
        user.UserName = model.UserName;
        user.Email = model.Email;
        user.EmailConfirmed = model.EmailConfirmed;
    
        await _userManager.UpdateAsync(user);
    
        return RedirectToAction("Index");
    }
    
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Details(string id)
    {
        if (string.IsNullOrEmpty(id))
            return NotFound();

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return NotFound();

        var roles = await _userManager.GetRolesAsync(user);

        var viewModel = new UserDetailsViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            EmailConfirmed = user.EmailConfirmed,
            Roles = roles.ToList(),
            ProjectsCreated = await _db.Projects
                .Where(p => p.FounderId == id)
                .Include(p => p.Tasks)
                .ToListAsync(),
            Memberships = await _db.ProjectMembers
                .Where(pm => pm.MemberId == id)
                .Include(pm => pm.Project)
                .ToListAsync(),
            RecentComments = await _db.Comments
                .Where(c => c.AuthorId == id)
                .Include(c => c.ProjectParent)
                .OrderByDescending(c => c.Date)
                .Take(10)
                .ToListAsync()
        };

        return View(viewModel);
    }
    
}

