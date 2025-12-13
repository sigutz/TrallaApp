using System.ComponentModel.DataAnnotations;

namespace DockerProject.Models;

public class Field
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required] [MaxLength(50)] public string Title { get; set; } = string.Empty;

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}