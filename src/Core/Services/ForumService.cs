using Core.Data;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Core.Services
{
    public class ForumService : IForumService
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ForumService(DataContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task CheckDb()
        {
            if (await IsDbEmpty())
            {
                await SeedDb();
            }
        }

        private async Task<bool> IsDbEmpty()
        {
            if (await _context.Subforums.CountAsync() > 0
                || await _context.Topics.CountAsync() > 0
                || await _context.Posts.CountAsync() > 0
                || await _context.Comments.CountAsync() > 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private async Task SeedDb()
        {
            List<ApplicationUser> users = new List<ApplicationUser>()
            {
                new ApplicationUser() { UserName = "boss", AuthAccessLevel = 4 },
                new ApplicationUser() { UserName = "patrick", AuthAccessLevel = 3 },
                new ApplicationUser() { UserName = "dude", AuthAccessLevel = 3 },
                new ApplicationUser() { UserName = "man", AuthAccessLevel = 1 },
                new ApplicationUser() { UserName = "dave", AuthAccessLevel = 1 },
                new ApplicationUser() { UserName = "john", AuthAccessLevel = 1 }
            };

            foreach (var user in users)
            {
                await _userManager.CreateAsync(user, "pass");
            }

            List<Subforum> subforumList = new List<Subforum>()
            {
                new Subforum() { Name = "sub1" },
                new Subforum() { Name = "sub2" }
            };

            List<Topic> topicList = new List<Topic>()
            {
                new Topic() { Name = "topic1", SubforumId = 1 },
                new Topic() { Name = "topic2", SubforumId = 1 },
                new Topic() { Name = "topic3", SubforumId = 2 },
                new Topic() { Name = "topic4", SubforumId = 2 }
            };

            await _context.Subforums.AddRangeAsync(subforumList);
            await _context.Topics.AddRangeAsync(topicList);
            // await _context.Posts.AddRangeAsync(postList);
            // await _context.Comments.AddRangeAsync(commentList);

            await _context.SaveChangesAsync();
        }
    }
}
