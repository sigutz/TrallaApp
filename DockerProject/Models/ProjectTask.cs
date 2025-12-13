using System.ComponentModel.DataAnnotations;

namespace DockerProject.Models;
public enum TaskStatusEnum
{
    ToDo,
    InProgress,
    Review,
    Done
}
public class ProjectTask
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required] [MaxLength(100)] public string Name { get; set; } = string.Empty;
    
    [MaxLength(500)] public string Description { get; set; } = string.Empty;
    
    public DateTime AssignedDate { get; set; }
    
    public DateTime DeadLine { get; set; }
    
    public DateTime DoneDate { get; set; }

    public TaskStatusEnum Status { get; set; } = TaskStatusEnum.ToDo;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();

    public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

}