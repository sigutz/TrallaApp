using DockerProject.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DockerProject.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectTask> Tasks { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Field> Fields { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<CommentVote> CommentsVotes { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<CommentVote>()
            .HasKey(cv => new { cv.UserId, cv.CommentId });

        builder.Entity<Project>()
            .HasMany(p => p.Members)
            .WithMany(u => u.MemberOf)
            .UsingEntity(j => j.ToTable("ProjectMembers"));

        builder.Entity<Project>()
            .HasMany(p => p.StarredBy)
            .WithMany(u => u.Starred)
            .UsingEntity(j => j.ToTable("ProjectStars"));

        builder.Entity<Project>()
            .HasOne(p => p.Founder)
            .WithMany()
            .HasForeignKey(p => p.FounderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Comment>()
            .HasOne(c => c.ProjectParent)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.ProjectParentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<ProjectTask>()
            .HasOne(t => t.ProjectParent)        
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectParentId) 
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Comment>()
            .HasOne(c => c.CommentParent)
            .WithMany()
            .HasForeignKey(c => c.CommentParentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserFriend>()
            .HasKey(uf => new { uf.UserId, uf.FriendId });

        builder.Entity<UserFriend>()
            .HasOne(uf => uf.User)
            .WithMany(u => u.Friends)
            .HasForeignKey(uf => uf.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<UserFriend>()
            .HasOne(uf => uf.Friend)
            .WithMany()
            .HasForeignKey(uf => uf.FriendId)
            .OnDelete(DeleteBehavior.Restrict);

        foreach (var entityType in builder.Model.GetEntityTypes())
        {
            foreach (var property in entityType.GetProperties())
            {
                if (property.ClrType == typeof(string))
                {
                    var maxLength = property.GetMaxLength();
                    if (maxLength == null)
                    {
                        if (property.IsKey() || property.IsForeignKey())
                        {
                            property.SetMaxLength(255);
                        }
                    }
                }
            }
        }
    }
}