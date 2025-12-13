using Microsoft.AspNetCore.Identity;

namespace DockerProject.Models;

public class ApplicationUser : IdentityUser
{
    public virtual ICollection<Project> MemberOf { get; set; } = new List<Project>();
    public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    public virtual ICollection<CommentVote> Votes { get; set; } = new List<CommentVote>();
}