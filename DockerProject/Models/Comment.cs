using System.ComponentModel.DataAnnotations;

namespace DockerProject.Models;

public class Comment
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();

    [MaxLength(5000)] public string Content { get; set; } = string.Empty;

    public DateTime Date { get; set; }
    
    public bool IsEdited { get; set; } = false;

    public string AuthorId { get; set; } 

    public virtual ApplicationUser? Author { get; set; }

    public string? CommentParentId { get; set; } 

    public virtual Comment? CommentParent { get; set; }

    public string? TaskParentId { get; set; } 

    public virtual ProjectTask? TaskParent { get; set; }

    public string? ProjectParentId { get; set; }

    public virtual Project? ProjectParent { get; set; }

    public virtual ICollection<CommentVote> Votes { get; set; } = new List<CommentVote>();
}