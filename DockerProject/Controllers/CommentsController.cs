using DockerProject.Data;
using DockerProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;

namespace DockerProject.Controllers;

public class CommentsController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ICompositeViewEngine _viewEngine; 
    private readonly ITempDataProvider _tempDataProvider;

    public CommentsController(
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

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> New(Comment comment)
    {
        try
        {
            comment.Date = DateTime.Now;
            var user = await _userManager.GetUserAsync(User);
            comment.AuthorId = user.Id;
        
            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            // Reload to include Author details for the View
            var commentForView = _db.Comments
                .Include(c => c.Author)
                .Include(c => c.Votes)
                .FirstOrDefault(c => c.Id == comment.Id);

            // Pass FounderId so "OP" badge works in the partial
            // Handles both Project Comments and Task Comments
            string? founderId = null;
            if (comment.ProjectParentId != null)
            {
                 var p = _db.Projects.Find(comment.ProjectParentId);
                 founderId = p?.FounderId;
            }
            else if (comment.TaskParentId != null) // If checking task parent
            {
                // You need to traverse Task -> Project to get founder
                // Assumes TaskParent is loaded or you query it:
                var t = _db.Tasks.Include(t => t.ProjectParent).FirstOrDefault(t => t.Id == comment.TaskParentId);
                founderId = t?.ProjectParent?.FounderId;
            }
            
            ViewData["ProjectFounderId"] = founderId;

            // Render Partial to String
            // Make sure the path is correct. If _CommentThread is in Views/Comments/ use this:
            string htmlString = await RenderViewAsync("~/Views/Comments/_CommentThread.cshtml", commentForView, true);

            return Json(new
            {
                success = true,
                html = htmlString,
                parentId = comment.CommentParentId,
                taskParentId = comment.TaskParentId,
                projectParentId = comment.ProjectParentId
            });
        }
        catch (Exception e)
        {
            return Json(new { success = false, message = e.Message });
        }
    }

    [HttpPost]
    [Authorize]
    public IActionResult Delete(string id)
    {
        Comment? comment = _db.Comments.Find(id);
        if (comment == null) return Json(new { success = false, message = "Not found" });

        if (comment.AuthorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
        {
            _db.Comments.Remove(comment);
            _db.SaveChanges();
            return Json(new { success = true });
        }
        return Json(new { success = false, message = "Unauthorized" });
    }

    [HttpPost]
    [Authorize]
    public IActionResult Edit(string id, string content)
    {
        Comment? comment = _db.Comments.Find(id);
        if (comment == null) return Json(new { success = false, message = "Not found" });

        if (comment.AuthorId == _userManager.GetUserId(User) || User.IsInRole("Admin"))
        {
            comment.Content = content;
            comment.IsEdited = true;
            _db.Comments.Update(comment);
            _db.SaveChanges();
            return Json(new { success = true, content = comment.Content });
        }
        return Json(new { success = false, message = "Unauthorized" });
    }

    [HttpPost]
    [Authorize]
    public IActionResult Vote(string commentId, bool isUpvote)
    {
        var currentUserId = _userManager.GetUserId(User);
        Comment? comment = _db.Comments.Find(commentId);

        if (comment is null) return NotFound();

        var existingVote = _db.CommentsVotes.FirstOrDefault(v => v.CommentId == commentId && v.UserId == currentUserId);

        if (existingVote != null)
        {
            if (existingVote.IsUpvote == isUpvote) _db.CommentsVotes.Remove(existingVote);
            else
            {
                existingVote.IsUpvote = isUpvote;
                _db.CommentsVotes.Update(existingVote);
            }
        }
        else
        {
            CommentVote vote = new CommentVote { CommentId = comment.Id, UserId = currentUserId, IsUpvote = isUpvote };
            _db.CommentsVotes.Add(vote);
        }

        _db.SaveChanges();
        
        var upVote = _db.CommentsVotes.Count(v => v.CommentId == commentId && v.IsUpvote);
        var downVote = _db.CommentsVotes.Count(v => v.CommentId == commentId && !v.IsUpvote);
        
        int userStatus = 0;
        var currentVote = _db.CommentsVotes.FirstOrDefault(v => v.CommentId == commentId && v.UserId == currentUserId);
        if (currentVote != null) userStatus = currentVote.IsUpvote ? 1 : -1;
        
        return Json(new {success = true, score = (upVote - downVote), UserStatus = userStatus});
    }

    // --- THIS IS THE MISSING FUNCTION ---
    private async Task<string> RenderViewAsync<TModel>(string viewName, TModel model, bool partial = false)
    {
        if (string.IsNullOrEmpty(viewName))
        {
            viewName = ControllerContext.ActionDescriptor.ActionName;
        }

        ViewData.Model = model;

        using (var writer = new StringWriter())
        {
            IViewEngine viewEngine = _viewEngine;
            ViewEngineResult viewResult = viewEngine.FindView(ControllerContext, viewName, !partial);

            if (viewResult.Success == false)
            {
                // Fallback: try to find it as a path if the name check failed
                viewResult = viewEngine.GetView(null, viewName, !partial);
            }

            if (viewResult.Success == false)
            {
                return $"A view with the name {viewName} could not be found";
            }

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