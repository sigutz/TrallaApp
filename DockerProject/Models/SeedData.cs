using DockerProject.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DotNetEnv;

namespace DockerProject.Models;

public class SeedData
{
    public static void Initializare(IServiceProvider serviceProvider)
    {
        using (var context = new ApplicationDbContext(
                   serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
        {
            if (context.Roles.Any())
                return;
            context.Roles.AddRange(
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d-56fd7210", Name = "Admin", NormalizedName = "Admin".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d-56fd7211", Name = "Editor", NormalizedName = "Editor".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af-483d-56fd7212", Name = "User", NormalizedName = "User".ToUpper()
                });

            var hasher = new PasswordHasher<ApplicationUser>();

            context.Users.AddRange(
                new ApplicationUser
                {
                    Id = "66d9ca59-241b-4995-be37-b1c0011b1dd1",
                    UserName = "admin@tralla.ro",
                    EmailConfirmed = true,
                    NormalizedEmail = "admin@tralla.ro".ToUpper(),
                    Email = "admin@tralla.ro",
                    NormalizedUserName = "admin@tralla.ro".ToUpper(),
                    PasswordHash = hasher.HashPassword(null, Environment.GetEnvironmentVariable("ADMIN_PASS"))
                },
                new ApplicationUser
                {
                    Id = "66d9ca59-241b-4995-be37-b1c0011b1dd2",
                    UserName = "editor@tralla.ro",
                    EmailConfirmed = true,
                    NormalizedEmail = "editor@tralla.ro".ToUpper(),
                    Email = "editor@tralla.ro",
                    NormalizedUserName = "editor@tralla.ro".ToUpper(),
                    PasswordHash = hasher.HashPassword(null, Environment.GetEnvironmentVariable("EDITOR_PASS"))
                },
                new ApplicationUser
                {
                    Id = "66d9ca59-241b-4995-be37-b1c0011b1dd3",
                    UserName = "user@tralla.ro",
                    EmailConfirmed = true,
                    NormalizedEmail = "user@tralla.ro".ToUpper(),
                    Email = "user@tralla.ro",
                    NormalizedUserName = "user@tralla.ro".ToUpper(),
                    PasswordHash = hasher.HashPassword(null, Environment.GetEnvironmentVariable("USER_PASS"))
                }
            );
            context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d-56fd7210",
                    UserId = "66d9ca59-241b-4995-be37-b1c0011b1dd1"
                }, new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d-56fd7211",
                    UserId = "66d9ca59-241b-4995-be37-b1c0011b1dd2"
                }, new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af-483d-56fd7212",
                    UserId = "66d9ca59-241b-4995-be37-b1c0011b1dd3"
                });
            context.SaveChanges();
        }
    }
}