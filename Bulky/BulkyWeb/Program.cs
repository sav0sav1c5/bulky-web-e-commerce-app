using Bulky.Utility;
using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository;
using Bulky.DataAccess.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// We can use it in teo different ways:
// First - With 'options' part that requires e-mail confirmation so you can try to 'Login'
// builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
// Second - User can 'Login' without confirmation e-mail. Also if we use addIdentity we will need to add one more thing that
// will add providers for tokens that are generated when user is created
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
// ! - Use addIdentity so you can pass both user and his 'Role'
// .AddEntityFrameworkStores<ApplicationDbContext>(); - add ALL tables that are needed for the 'Identity' 

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = $"/Identity/Account/Login";
    options.LogoutPath = $"/Identity/Account/Logout";
    options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
});

// Adding authentication for Facebook
builder.Services.AddAuthentication().AddFacebook(options => {
    options.AppId = "1734738443748849";
    options.AppSecret = "42381240f538a5779befd70dfe7e6df9";
});

// Distributed memory cache
builder.Services.AddDistributedMemoryCache();

// Session - this is just adding session to services
builder.Services.AddSession(options =>
{
    options.IOTimeout = TimeSpan.FromMinutes(100);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Registered AddRazorPages
builder.Services.AddRazorPages();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Check for routes in URL, map them and send to [Area]/[Controller]/[Action]
app.UseRouting();
// Check if credentials are ok/valid
app.UseAuthentication();
// Check if user has valid credentials has permission to access some resource
app.UseAuthorization();
// Session - also need to be added in request pipeline
app.UseSession();
// Use all Razor Pages of application and map them like endpoints so they can respond to HTTP request
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{area=Customer}/{controller=Home}/{action=Index}/{id?}");

app.Run();
