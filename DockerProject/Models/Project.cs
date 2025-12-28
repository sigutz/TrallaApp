using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DockerProject.Models;

public class Project
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required] [MaxLength(100)] public string Title { get; set; } = string.Empty;
    
    [MaxLength(500)] public string Description { get; set; } = string.Empty;

    public DateTime CreatedDate { get; set; }

    public string FounderId { get; set; }
    public virtual ApplicationUser Founder { get; set; }
    
    public virtual ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();

    public virtual ICollection<ApplicationUser> StarredBy { get; set; } = new List<ApplicationUser>();

    public virtual ICollection<Field> Fields { get; set; } = new List<Field>();

    public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
}