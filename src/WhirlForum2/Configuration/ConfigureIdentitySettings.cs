using Microsoft.AspNetCore.Identity;

namespace WhirlForum2.Configuration
{
    public static class ConfigureIdentitySettings
    {
        public static IServiceCollection AddIdentitySettings(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
            });

            return services;
        }
    }
}
