using Core.Data;
using Core.Entities;
using Core.Interfaces;
using Core.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Web.Configuration;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IForumService, ForumService>();
builder.Services.AddTransient<ISubforumService, SubforumService>();
builder.Services.AddTransient<ITopicService, TopicService>();
builder.Services.AddTransient<IPostService, PostService>();
builder.Services.AddTransient<ICommentService, CommentService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<IUserManagementService, UserManagementService>();
builder.Services.AddTransient<IUserManagementService, UserManagementService>();
builder.Services.AddTransient<IPostManagementService, PostManagementService>();
builder.Services.AddTransient<ICommentManagementService, CommentManagementService>();

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<DataContext>();

builder.Services.AddIdentitySettings();
builder.Services.AddCookieSettings();

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