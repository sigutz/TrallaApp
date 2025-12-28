namespace DockerProject.Models;
public enum ProjectMemberStatus
{
    Pending,
    Banned,
    Accepted
}
public class ProjectMember
{
    public string ProjectId { get; set; }
    public virtual Project Project { get; set; } = null!;
    
    public string MemberId { get; set; } 
    public virtual ApplicationUser Member { get; set; } = null!;

    public ProjectMemberStatus Status { get; set; } = ProjectMemberStatus.Pending;
}