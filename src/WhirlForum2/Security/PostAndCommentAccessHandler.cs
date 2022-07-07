using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using WhirlForum2.Entities;
using WhirlForum2.Models;

namespace WhirlForum2.Security
{
    public class PostAndCommentAccessHandler : AuthorizationHandler<PostAndCommentAccessRequirement, PostAndCommentAccessModel>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PostAndCommentAccessRequirement requirement, PostAndCommentAccessModel postAndCommentAccessModel)
        {
            bool hasAccess = true;
            var currentUserClaims = context.User.Claims;

            if (context.User.IsInRole("Root"))
            {
                hasAccess = true;
            }
            else if (context.User.IsInRole("Admin"))
            {
                if (postAndCommentAccessModel.EditUserRoles.Any(r => r == "Root" || r == "Admin"))
                {
                    hasAccess = false;
                }
            }
            else if (context.User.IsInRole("Moderator"))
            {
                if (postAndCommentAccessModel.EditUserRoles.Any(r => r == "Root" || r == "Admin" || r == "Moderator"))
                {
                    hasAccess = false;
                }
                else
                {
                    if (currentUserClaims.Any(c => c.Type == "ModAccess_" + postAndCommentAccessModel.SubforumId.ToString() && c.Value == "true"))
                    {
                        hasAccess = true;
                    }
                    else
                    {
                        hasAccess = false;
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
