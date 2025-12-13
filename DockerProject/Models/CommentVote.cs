using System.ComponentModel.DataAnnotations;

namespace DockerProject.Models;

public class CommentVote
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();
    public bool IsUpvote { get; set; }
    
    public string UserId { get; set; }
    public virtual ApplicationUser User { get; set; }
    public string CommentId { get; set; }
    public virtual Comment Comment { get; set; }
}