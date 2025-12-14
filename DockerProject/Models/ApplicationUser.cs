using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace DockerProject.Models;

public enum Lang
{

    [Description("ro-RO")]
    [Display(Name = "Română")]
    Romanian,

    [Description("en-US")]
    English,

    [Description("es-ES")]
    Spanish,

    [Description("it-IT")]
    Italian,

    [Description("de-DE")]
    German,

    [Description("bg-BG")]
    Bulgarian,

    [Description("hu-HU")]
    Hungarian,

    [Description("pl-PL")]
    Polish,

    [Description("uk-UA")]
    Ukrainian,

    [Description("ru-RU")]
    Russian,

    [Description("fr-FR")]
    French
}

public enum Theme
{
    Light,
    Dark
}
public class ApplicationUser : IdentityUser
{
    public Lang PreferredLanguage { get; set; } = Lang.Romanian;
    
    public Theme PreferredTheme { get; set; } = Theme.Light;
    
    public string? ProfilePictureUrl { get; set; }

    public virtual ICollection<UserFriend> Friends { get; set; } = new List<UserFriend>();
    
    public virtual ICollection<Project> MemberOf { get; set; } = new List<Project>();
    
    public virtual ICollection<Project> Starred { get; set; } = new List<Project>();
    
    public virtual ICollection<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();
    
    public virtual ICollection<CommentVote> Votes { get; set; } = new List<CommentVote>();
}

public class UserFriend
{
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public string FriendId { get; set; }
    public ApplicationUser Friend { get; set; }
}