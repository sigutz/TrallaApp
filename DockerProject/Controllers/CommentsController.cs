using DockerProject.Data;
using DockerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace DockerProject.Controllers;

public class CommentsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public CommentsController(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        _db = context;
        _userManager = userManager;
        _roleManager = roleManager;
    }

    [HttpPost]
    [Authorize(Roles = "User,Editor,Admin")]
    public IActionResult New(Comment comment)
    {
        comment.Date = DateTime.Now;
        comment.AuthorId = _userManager.GetUserId(User);

        try
        {
            _db.Comments.Add(comment);
            _db.SaveChanges();
            return Redirect($"/Projects/Show/{comment.ProjectParentId}");
        }
        catch (Exception e)
        {
            TempData["message"] = e.Message;
            TempData["messageType"] = "alert-danger";
            return Redirect("/Projects/Index");
        }
    }

    [HttpPost]
    [Authorize(Roles = "User,Editor,Admin")]
    public IActionResult Delete(string id)
    {
        Comment? comment = _db.Comments.Find(id);
        if (comment is null)
            return NotFound();

        if (comment.AuthorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
        {
            _db.Comments.Remove(comment);
            _db.SaveChanges();
        }
        else
        {
            TempData["message"] = "Nu aveti dreptul sa stergeti comentariul";
            TempData["messageType"] = "alert-danger";
        }

        return Redirect($"/Projects/Show/{comment.ProjectParentId}");
    }

    [Authorize(Roles = "User,Editor,Admin")]
    public IActionResult Edit(string id)
    {
        Comment? comment = _db.Comments.Find(id);
        if (comment is null)
            return NotFound();

        if (comment.AuthorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
        {
            return View(comment);
        }
        else
        {
            TempData["message"] = "Nu aveti dreptul sa editati comentariul";
            TempData["messageType"] = "alert-danger";
            return Redirect($"/Projects/Show/{comment.ProjectParentId}");
        }
    }

    [HttpPost]
    [Authorize]
    public IActionResult Vote(string commentId, bool isUpvote) // 1. Fixed parameter name to match View
    {
        var currentUserId = _userManager.GetUserId(User);
    
        Comment? comment = _db.Comments.Find(commentId);

        if (comment is null)
        {
            return NotFound();
        }

        var existingVote = _db.CommentsVotes
            .FirstOrDefault(v => v.CommentId == commentId && v.UserId == currentUserId);

        if (existingVote != null)
        {
            if (existingVote.IsUpvote == isUpvote)
            {
                _db.CommentsVotes.Remove(existingVote);
            }
            else
            {
                existingVote.IsUpvote = isUpvote;
                _db.CommentsVotes.Update(existingVote);
            }
        }
        else
        {
            CommentVote vote = new CommentVote
            {
                CommentId = comment.Id,
                UserId = currentUserId,
                IsUpvote = isUpvote
            };
            _db.CommentsVotes.Add(vote);
        }

        _db.SaveChanges();
        return Redirect($"/Projects/Show/{comment.ProjectParentId}");
    }
}