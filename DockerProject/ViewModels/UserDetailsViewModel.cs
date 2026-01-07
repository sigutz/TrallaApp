using DockerProject.Models;

namespace DockerProject.ViewModels;

public class UserDetailsViewModel
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<Project> ProjectsCreated { get; set; } = new();
    public List<ProjectMember> Memberships { get; set; } = new();
    public List<Comment> RecentComments { get; set; } = new();
}