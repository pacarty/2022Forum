using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WhirlForum2.Configuration;
using WhirlForum2.Data;
using WhirlForum2.Entities;
using WhirlForum2.Security;
using WhirlForum2.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IForumService, ForumService>();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddIdentitySettings();
builder.Services.AddCookieSettings();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("UserEditDeletePolicy",
        policy => policy.AddRequirements(new UserRequirement()));
});

builder.Services.AddSingleton<IAuthorizationHandler, UserHandler>();

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

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapDefaultControllerRoute();

app.Run();
