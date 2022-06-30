using Microsoft.AspNetCore.Authorization;

namespace WhirlForum2.Security
{
    public class ModeratorAccessHandler : AuthorizationHandler<ModeratorAccessRequirement, IList<string>>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ModeratorAccessRequirement requirement, IList<string> editUserRoles)
        {
            bool hasAccess = true;

            if (context.User.IsInRole("Root"))
            {
                hasAccess = true;
            }
            else if (context.User.IsInRole("Admin"))
            {
                if (editUserRoles.Any(r => r == "Root" || r == "Admin"))
                {
                    hasAccess = false;
                }
            }
            else
            {
                hasAccess = false;
            }

            if (hasAccess)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

    }
}
