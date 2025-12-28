using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DockerProject.Data;
using DockerProject.Models;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);

Env.Load(); //pt siguranta

// creaza connextion string ul
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// se conecteaza la baza de date
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 4))));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// se adauga identity
builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// se adauga rolurile de baza (din SeedData) la prima rulare daca nu sunt in baza de date
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    SeedData.Initializare(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();


app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "projects",
    pattern: "{controller=Projects}/{action=Index}/{userid?}").WithStaticAssets();

app.MapRazorPages()
    .WithStaticAssets();

app.Run();