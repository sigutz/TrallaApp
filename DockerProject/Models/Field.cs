using System.ComponentModel.DataAnnotations;

namespace DockerProject.Models;

public class Field
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required] [MaxLength(50)] public string Title { get; set; } = string.Empty;
    
    [Required] [StringLength(8)] public string HexColor { get; set; } = "00000000";

    public virtual ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}