using DockerProject.Models;

namespace DockerProject.ViewModels;

public class AdminDashboardViewModel
{
    public int TotalUsers { get; set; }
    public int TotalProjects { get; set; }
    public int TotalTasks { get; set; }
    public int TotalComments { get; set; }
    public int CompletedTasks { get; set; }
    public int InProgressTasks { get; set; }
    public int PendingMemberships { get; set; }
    public List<Project> RecentProjects { get; set; } = new();
    public List<ApplicationUser> RecentUsers { get; set; } = new();
    public List<Comment> RecentComments { get; set; } = new();
}