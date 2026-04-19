using Menulo.Infrastructure.Extensions;
using Menulo.Infrastructure.Persistence;
using Menulo.Infrastructure.Persistence.Seed;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;

var projectRoot = ResolveProjectRoot();
var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = projectRoot,
    WebRootPath = Path.Combine(projectRoot, "wwwroot")
});

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllersWithViews();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, "uploads", "logos"));
Directory.CreateDirectory(Path.Combine(app.Environment.WebRootPath, "uploads", "menu-items"));

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<MenuloDbContext>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Menulo.Infrastructure.Identity.ApplicationUser>>();
    await DemoDataSeeder.SeedAsync(dbContext, userManager);
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "owner-area",
    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "public-menu",
    pattern: "{restaurantSlug}/menu",
    defaults: new { controller = "PublicMenu", action = "Index" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();

static string ResolveProjectRoot()
{
    var currentDirectory = AppContext.BaseDirectory;
    var candidate = Path.GetFullPath(Path.Combine(currentDirectory, "..", "..", ".."));

    if (Directory.Exists(Path.Combine(candidate, "Views")) &&
        Directory.Exists(Path.Combine(candidate, "wwwroot")) &&
        File.Exists(Path.Combine(candidate, "Menulo.Web.csproj")))
    {
        return candidate;
    }

    return Directory.GetCurrentDirectory();
}
