using System.ComponentModel.DataAnnotations;

namespace DockerProject.Models;

public class Tag
{
    [Key] public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required] [MaxLength(50)] public string Name { get; set; } = string.Empty;

    [Required] [StringLength(8)] public string HexColor { get; set; } = "00000000";

    public virtual ICollection<Field> Fields { get; set; } = new List<Field>();
    
    public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
}