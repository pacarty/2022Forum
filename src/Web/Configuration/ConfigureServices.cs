using Core.Interfaces;
using Core.Services;

namespace Web.Configuration
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddTransient<IForumService, ForumService>();
            services.AddTransient<ISubforumService, SubforumService>();
            services.AddTransient<ITopicService, TopicService>();
            services.AddTransient<IPostService, PostService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddTransient<IUserManagementService, UserManagementService>();
            services.AddTransient<IUserManagementService, UserManagementService>();
            services.AddTransient<IPostManagementService, PostManagementService>();
            services.AddTransient<ICommentManagementService, CommentManagementService>();

            return services;
        }
    }
}
