using Microsoft.AspNetCore.Identity;

namespace Web.Configuration
{
    public static class ConfigureIdentitySettings
    {
        public static IServiceCollection AddIdentitySettings(this IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = false;
            });

            return services;
        }
    }
}
