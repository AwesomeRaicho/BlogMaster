using BlogMaster.Core.Models.Identity;
using BlogMaster.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// DB ENTITY AND IDENTITY SERVICES ~ START
builder.Services.AddDbContext<EntityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<EntityDbContext>()
    .AddDefaultTokenProviders();
// DB ENTITY AND IDENTITY SERVICES ~ END











var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();