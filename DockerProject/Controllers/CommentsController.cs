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
    [Authorize]
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
    [Authorize]
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

    [Authorize]
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

        // verificam daca exista deja un vot in t.a.
        var existingVote = _db.CommentsVotes
            .FirstOrDefault(v => v.CommentId == commentId && v.UserId == currentUserId);

        if (existingVote != null) // cazul in care e deja in baza de date
        {
            
            if (existingVote.IsUpvote == isUpvote) // verificam daca si a luat votul
            {
                _db.CommentsVotes.Remove(existingVote);
            }
            else // daca nu schimbam votul
            {
                existingVote.IsUpvote = isUpvote;
                _db.CommentsVotes.Update(existingVote);
            }
        }
        else // cazul in care nu este votul in baza de date (il adaugam);
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
        
        var upVote = _db.CommentsVotes.Count(v => v.CommentId == commentId && v.IsUpvote);
        var downVote = _db.CommentsVotes.Count(v => v.CommentId == commentId && v.IsUpvote == false);
        var newScore = upVote - downVote;

        // 0 -> None, 1 -> Up, -1 -> Down
        int userStatus = 0;
        var currentVote = _db.CommentsVotes.FirstOrDefault(v => v.CommentId == commentId && v.UserId == currentUserId);
        if (currentVote != null)
        {
            userStatus = currentVote.IsUpvote ? 1 : -1;
        }
        
        
        return Json(new {success = true, score = newScore, UserStatus = userStatus});
    }
}