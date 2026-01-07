namespace DockerProject.ViewModels;

public class UserViewModel
{
    public string Id { get; set; }
    public string UserName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTimeOffset? LockoutEnd { get; set; }  
    public bool IsActive { get; set; }              
    public List<string> Roles { get; set; } = new();
    public int ProjectsCreated { get; set; }
    public int ProjectsMember { get; set; }
}