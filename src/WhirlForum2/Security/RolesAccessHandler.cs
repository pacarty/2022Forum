using Microsoft.AspNetCore.Authorization;
using WhirlForum2.Models;

namespace WhirlForum2.Security
{
    public class RolesAccessHandler : AuthorizationHandler<RolesAccessRequirement, RolesAccessModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, RolesAccessRequirement requirement, RolesAccessModel rolesAccessModel)
        {
            bool hasAccess = true;

            if (context.User.IsInRole("Root"))
            {
                hasAccess = true;
            }
            else if (context.User.IsInRole("Admin"))
            {
                if (rolesAccessModel.EditUserRoles.Any(r => r == "Root" || r == "Admin"))
                {
                    hasAccess = false;
                }

                foreach (var roleModel in rolesAccessModel.RolesToEdit)
                {
                    if (roleModel.RoleName == "Root" || roleModel.RoleName == "Admin")
                    {
                        hasAccess = false;
                        break;
                    }
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
