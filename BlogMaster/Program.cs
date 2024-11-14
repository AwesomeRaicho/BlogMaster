using BlogMaster.Core.Contracts;
using BlogMaster.Core.Models.Identity;
using BlogMaster.Core.Services;
using BlogMaster.Infrastructure.DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BlogMaster.Core.Models;
using Stripe;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddControllers(options =>
//{
//    options.MaxIAsyncEnumerableBufferLimit = int.MaxValue;
//});

//builder.Services.Configure<KestrelServerOptions>(options =>
//{
//    options.Limits.KeepAliveTimeout = TimeSpan.FromSeconds(120);
//    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
//});






builder.Configuration.AddJsonFile("Secret.json", optional: true, reloadOnChange: true);
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));





builder.Services.AddControllersWithViews();

// DB ENTITY AND IDENTITY SERVICES ~ START
builder.Services.AddDbContext<EntityDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultConnection"]);
});

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(option =>
{
    option.User.RequireUniqueEmail = true;
    option.SignIn.RequireConfirmedEmail = true;
})
    .AddEntityFrameworkStores<EntityDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddScoped(typeof(IRepository<>), typeof(GeneralRepository<>));

builder.Services.AddScoped<IBlogRepository, BlogRepository>();


// DB ENTITY AND IDENTITY SERVICES ~ END

// BUSINESS LAYER (SERVICES) ~ START

builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IIdentityService, IdentityService>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IStripeService, StripeService>();
builder.Services.AddScoped<IAppSubscriptionService, AppSubscriptionService>();
builder.Services.AddScoped<CookieService>();

StripeConfiguration.ApiKey = builder.Configuration["Stripe:SecretKey"];

// BUSINESS LAYER (SERVICES) ~ END

var app = builder.Build();




app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();