namespace Web.Configuration
{
    public static class ConfigureCookieSettings
    {
        public static IServiceCollection AddCookieSettings(this IServiceCollection services)
        {
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Signin";
            });

            return services;
        }
    }
}
