using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WhirlForum2.Entities;
using WhirlForum2.Models;

namespace WhirlForum2.Security
{
    public class UserAccessHandler : AuthorizationHandler<UserAccessRequirement, UserAccesModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, UserAccessRequirement requirement, UserAccesModel userAccesModel)
        {
            bool hasAccess = true;
            var currentUserClaims = context.User.Claims;

            if (context.User.IsInRole("Root"))
            {
                hasAccess = true;
            }
            else if (context.User.IsInRole("Admin"))
            {
                if (userAccesModel.EditUserRoles.Any(r => r == "Root" || r == "Admin"))
                {
                    hasAccess = false;
                }
            }
            else if (context.User.IsInRole("Moderator"))
            {
                if (userAccesModel.EditUserRoles.Any(r => r == "Root" || r == "Admin" || r == "Moderator"))
                {
                    hasAccess = false;
                }
                else
                {
                    foreach (var subforum in userAccesModel.SubforumAccess)
                    {
                        if (currentUserClaims.Any(c => c.Type == "ModAccess_" + subforum.SubforumId.ToString() && c.Value == "true"))
                        {
                            continue;
                        }
                        else
                        {
                            hasAccess = false;
                            break;
                        }
                    }
                }
            }
            else
            {
                hasAccess=false;
            }

            if (hasAccess)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }

    }
}
