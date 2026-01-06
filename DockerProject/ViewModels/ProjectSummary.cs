namespace DockerProject.ViewModels;

public class ProjectSummary
{
    public string Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TaskCount { get; set; }
    public int CompletedTaskCount { get; set; }
    public DateTime? NextDeadline { get; set; }
}