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

    // se adauga toate tabelele ce vor fi folosite pentru a se migra in baza de date
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

        // se creeaza cheia pentru tabelul asociativ (creat manual) care contorizeaza Voturile
        builder.Entity<CommentVote>()
            .HasKey(cv => new { cv.UserId, cv.CommentId });

        //tabelul asociativ intre useri si din ce proiecte fac parte
        builder.Entity<Project>()
            .HasMany(p => p.Members)
            .WithMany(u => u.MemberOf)
            .UsingEntity(j => j.ToTable("ProjectMembers"));

        // tabelul asociativ care contorizeaza proiectele puse de user la stared
        builder.Entity<Project>()
            .HasMany(p => p.StarredBy)
            .WithMany(u => u.Starred)
            .UsingEntity(j => j.ToTable("ProjectStars"));

        // tabelul asociativ dintre un user si proiectele lui
        builder.Entity<Project>()
            .HasOne(p => p.Founder)
            .WithMany()
            .HasForeignKey(p => p.FounderId)
            .OnDelete(DeleteBehavior.Restrict); // nu se sterge userul daca se sterge un proiect

        
        builder.Entity<Comment>()
            .HasOne(c => c.ProjectParent)
            .WithMany(p => p.Comments)
            .HasForeignKey(c => c.ProjectParentId)
            .OnDelete(DeleteBehavior.Cascade); //nu se sterge proiectul daca se sterge un comentariu din el

        builder.Entity<ProjectTask>()
            .HasOne(t => t.ProjectParent)        
            .WithMany(p => p.Tasks)
            .HasForeignKey(t => t.ProjectParentId) 
            .OnDelete(DeleteBehavior.Cascade); //nu se sterge proiectul daca se sterge un task din el

        builder.Entity<Comment>()
            .HasOne(c => c.CommentParent)
            .WithMany()
            .HasForeignKey(c => c.CommentParentId)
            .OnDelete(DeleteBehavior.Cascade); // nu se sterge comentariul parinte daca se sterge unul din copiii lui

        // creaza cheia pentru t.a. (creat manual) dintre un user si prietenii lui
        builder.Entity<UserFriend>()
            .HasKey(uf => new { uf.UserId, uf.FriendId });

        builder.Entity<UserFriend>()
            .HasOne(uf => uf.User)
            .WithMany(u => u.Friends)
            .HasForeignKey(uf => uf.UserId)
            .OnDelete(DeleteBehavior.Restrict); // nu se sterg userii daca nu mai sunt prieteni

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