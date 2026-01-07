namespace DockerProject.ViewModels;

public class EditUserViewModel
{
    public string Id { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool EmailConfirmed { get; set; }
    public List<string> CurrentRoles { get; set; } = new();
    public List<string> AllRoles { get; set; } = new();
}