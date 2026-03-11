using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SDHRM.Data;
using SDHRM.Models;
using SDHRM.Models.Process;
using SDHRM.Models.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.AddTransient<EmailSender>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.ConfigureApplicationCookie(options =>
{
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.LoginPath = "/Identity/Account/Login";
});

builder.Services.AddAuthorization(options =>
{
    foreach (var permission in SDHRM.Models.Constants.Permissions.GenerateAllPermissions())
    {
        options.AddPolicy(permission, policy =>
            policy.RequireClaim("Permission", permission));
    }
});

// 2) Identity + Roles (chỉ dùng 1 cái này)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.SignIn.RequireConfirmedEmail = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
