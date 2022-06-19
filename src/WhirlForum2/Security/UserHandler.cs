using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace WhirlForum2.Security
{
    public class UserHandler : AuthorizationHandler<UserRequirement, string>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserRequirement requirement, string commentUserId)
        {
            string loggedInUserId = context.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier).Value;

            if (context.User.IsInRole("User") && loggedInUserId == commentUserId)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

    }
}
