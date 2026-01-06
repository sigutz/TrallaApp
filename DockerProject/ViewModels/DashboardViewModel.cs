namespace DockerProject.ViewModels;

public class DashboardViewModel
{
    public List<ProjectSummary> ActiveProjects { get; set; } = new();
    public Dictionary<string, List<TaskSummary>> TasksByStatus { get; set; } = new();
    public List<TaskSummary> UpcomingDeadlines { get; set; } = new();
    public string? CurrentFilter { get; set; }
}