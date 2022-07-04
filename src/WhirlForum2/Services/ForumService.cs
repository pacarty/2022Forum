using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WhirlForum2.Data;
using WhirlForum2.Entities;
using WhirlForum2.Models;

namespace WhirlForum2.Services
{
    public class ForumService : IForumService
    {
        private readonly DataContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ForumService(DataContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedDb()
        {
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

            await _roleManager.CreateAsync(new IdentityRole { Name = "User" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "Moderator" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "Admin" });
            await _roleManager.CreateAsync(new IdentityRole { Name = "Root" });

            await _context.Subforums.AddRangeAsync(subforumList);
            await _context.Topics.AddRangeAsync(topicList);
            // await _context.Posts.AddRangeAsync(postList);
            // await _context.Comments.AddRangeAsync(commentList);

            await _context.SaveChangesAsync();

        }

        public async Task<List<SubforumModel>> GetSubforumModels(int postsToDisplay)
        {
            List<SubforumModel> subforumModels = new List<SubforumModel>();

            foreach (Subforum subforum in await _context.Subforums.ToListAsync())
            {
                List<TopicModel> topicModels = new List<TopicModel>();

                foreach (Topic topic in await _context.Topics.Where(t => t.SubforumId == subforum.Id).ToListAsync())
                {
                    List<PostModel> postModels = new List<PostModel>();

                    foreach (Post post in await _context.Posts.Where(p => p.TopicId == topic.Id).Take(postsToDisplay).ToListAsync())
                    {
                        postModels.Add(new PostModel
                        {
                            Id = post.Id,
                            Name = post.Name
                        });
                    }

                    topicModels.Add(new TopicModel
                    {
                        Id = topic.Id,
                        Name = topic.Name,
                        PostModels = postModels
                    });
                }

                subforumModels.Add(new SubforumModel
                {
                    Id = subforum.Id,
                    Name = subforum.Name,
                    TopicModels = topicModels
                });
            }

            return subforumModels;
        }

        public async Task<TopicModel> GetTopicModel(int topicId, int pageIndex, int postsOnPage, int commentsToDisplay)
        {
            Topic topic = await _context.Topics.FindAsync(topicId);
            TopicModel topicModel = new TopicModel
            {
                Id = topic.Id,
                Name = topic.Name
            };

            List<Post> posts = await _context.Posts.Where(p => p.TopicId == topicId)
                .Skip(postsOnPage * (pageIndex - 1))
                .Take(postsOnPage)
                .ToListAsync();

            int totalItems = await _context.Posts.Where(p => p.TopicId == topicId).CountAsync();

            List<PostModel> postModels = new List<PostModel>();

            foreach (Post post in posts)
            {
                List<CommentModel> commentModels = new List<CommentModel>();

                foreach(Comment comment in await _context.Comments.Where(c => c.PostId == post.Id).Take(commentsToDisplay).ToListAsync())
                {
                    var commentUser = await _context.Users.FindAsync(comment.UserId);

                    var commentUserModel = new UserModel
                    {
                        UserId = commentUser.Id,
                        Username = commentUser.UserName
                    };

                    commentModels.Add(new CommentModel
                    {
                        Id = comment.Id,
                        Content = comment.Content,
                        UserModel = commentUserModel
                    });
                }

                var postUser = await _context.Users.FindAsync(post.UserId);

                var postUserModel = new UserModel
                {
                    UserId = postUser.Id,
                    Username = postUser.UserName
                };

                postModels.Add(new PostModel
                {
                    Id = post.Id,
                    Name = post.Name,
                    CommentModels = commentModels,
                    UserModel = postUserModel
                });
            }

            topicModel.PostModels = postModels;

            topicModel.PaginationInfo = new PaginationInfo
            {
                TotalItems = totalItems,
                ItemsPerPage = postsOnPage,
                CurrentPage = pageIndex,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / postsOnPage)).ToString()),
                Previous = pageIndex - 1,
                Next = pageIndex + 1
            };

            return topicModel;
        }

        public async Task<PostModel> GetPostModel(int postId, int pageIndex, int commentsOnPage, string? currentUserId)
        {
            Post post = await _context.Posts.FindAsync(postId);

            var postUser = await _context.Users.FindAsync(post.UserId);

            var postUserModel = new UserModel
            {
                UserId = postUser.Id,
                Username = postUser.UserName
            };

            PostModel postModel = new PostModel
            {
                Id = post.Id,
                Name = post.Name,
                UserModel = postUserModel
            };

            List<Comment> comments = await _context.Comments.Where(c => c.PostId == postId)
                .Skip(commentsOnPage * (pageIndex - 1))
                .Take(commentsOnPage)
                .ToListAsync();

            // if the count is zero then something must have gone wrong (eg the first comment on a new page was deleted,
            // and so the comments are trying to be loaded into a non-existant page).
            // here we will change the pageIndex to 1 so it acts like it is on the first page of the post.
            // note: i'm simply setting the pageIndex to 1 so that previous and next values are correct and it doesn't confuse things.
            if (comments.Count == 0)
            {
                pageIndex = 1;

                comments = await _context.Comments.Where(c => c.PostId == postId)
                    .Skip(commentsOnPage * (pageIndex - 1))
                    .Take(commentsOnPage)
                    .ToListAsync();
            }

            int totalItems = await _context.Comments.Where(c => c.PostId == postId).CountAsync();

            List<CommentModel> commentsModel = new List<CommentModel>();
            
            foreach (Comment comment in comments)
            {
                var commentUser = await _context.Users.FindAsync(comment.UserId);
                bool isUser = false;

                if (currentUserId == commentUser.Id)
                {
                    isUser = true;
                }

                var commentUserModel = new UserModel
                {
                    UserId = commentUser.Id,
                    Username = commentUser.UserName
                };

                commentsModel.Add(new CommentModel
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    UserModel = commentUserModel,
                    IsUser = isUser
                });
            }

            postModel.CommentModels = commentsModel;

            postModel.PaginationInfo = new PaginationInfo
            {
                TotalItems = totalItems,
                ItemsPerPage = commentsOnPage,
                CurrentPage = pageIndex,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / commentsOnPage)).ToString()),
                Previous = pageIndex - 1,
                Next = pageIndex + 1
            };

            postModel.NewCommentModel = new NewCommentModel
            {
                PostId = postId,
                CurrentPage = pageIndex
            };

            postModel.EditCommentModel = new EditCommentModel
            {
                PostId = postId,
                CurrentPage = pageIndex
            };

            return postModel;
        }

        public async Task AddPost(NewPostModel newPostModel)
        {
            // post variable declared so we can use post.Id to set new Comment PostId to this variable's Id.
            // after we call savechangesasync() we are able to use this variables primary key (Id).
            Post post = new Post
            {
                Name = newPostModel.Title,
                TopicId = newPostModel.TopicId,
                UserId = newPostModel.UserId
            };

            await _context.Posts.AddAsync(post);
            await _context.SaveChangesAsync();

            await _context.Comments.AddAsync(
                new Comment
                {
                    Content = newPostModel.Content,
                    PostId = post.Id,
                    UserId = newPostModel.UserId
                });

            await _context.SaveChangesAsync();
        }

        public async Task AddComment(NewCommentModel newCommentModel)
        {
            await _context.Comments.AddAsync(
                new Comment
                {
                    PostId = newCommentModel.PostId,
                    Content = newCommentModel.Content,
                    UserId = newCommentModel.UserId
                });

            await _context.SaveChangesAsync();
        }

        public async Task<PostManagementModel> GetPostManagementModel(int pageIndex, int postsOnPage)
        {
            List<Post> posts = await _context.Posts
                .Skip(postsOnPage * (pageIndex - 1))
                .Take(postsOnPage)
                .ToListAsync();

            if (posts.Count == 0)
            {
                pageIndex = 1;

                posts = await _context.Posts
                    .Skip(postsOnPage * (pageIndex - 1))
                    .Take(postsOnPage)
                    .ToListAsync();
            }

            int totalItems = await _context.Posts.CountAsync();

            List<PostModel> postModels = new List<PostModel>();

            foreach (Post post in posts)
            {
                var postUser = await _context.Users.FindAsync(post.UserId);

                var postUserModel = new UserModel
                {
                    UserId = postUser.Id,
                    Username = postUser.UserName
                };

                postModels.Add(new PostModel
                {
                    Id = post.Id,
                    Name = post.Name,
                    UserModel = postUserModel
                });
            }

            PostManagementModel postManagementModel = new PostManagementModel
            {
                PostModels = postModels
            };

            postManagementModel.PaginationInfo = new PaginationInfo
            {
                TotalItems = totalItems,
                ItemsPerPage = postsOnPage,
                CurrentPage = pageIndex,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / postsOnPage)).ToString()),
                Previous = pageIndex - 1,
                Next = pageIndex + 1
            };

            postManagementModel.EditPostModel = new EditPostModel
            {
                CurrentPage = pageIndex
            };

            return postManagementModel;
        }

        public async Task<CommentManagementModel> GetCommentManagementModel(int? postId, int pageIndex, int commentsOnPage)
        {
            // need to initialise these for some reason
            List<Comment> comments = new List<Comment>();
            int totalItems = 0;

            if (postId == null)
            {
                comments = await _context.Comments
                .Skip(commentsOnPage * (pageIndex - 1))
                .Take(commentsOnPage)
                .ToListAsync();

                totalItems = await _context.Comments.CountAsync();

                if (comments.Count == 0)
                {
                    pageIndex = 1;

                    comments = await _context.Comments
                        .Skip(commentsOnPage * (pageIndex - 1))
                        .Take(commentsOnPage)
                        .ToListAsync();

                    totalItems = await _context.Comments.CountAsync();
                }
            }
            else
            {
                comments = await _context.Comments.Where(c => c.PostId == postId)
                .Skip(commentsOnPage * (pageIndex - 1))
                .Take(commentsOnPage)
                .ToListAsync();

                totalItems = await _context.Comments.Where(c => c.PostId == postId).CountAsync();

                if (comments.Count == 0)
                {
                    pageIndex = 1;

                    comments = await _context.Comments.Where(c => c.PostId == postId)
                        .Skip(commentsOnPage * (pageIndex - 1))
                        .Take(commentsOnPage)
                        .ToListAsync();

                    totalItems = await _context.Comments.Where(c => c.PostId == postId).CountAsync();
                }
            }

            List<CommentModel> commentModels = new List<CommentModel>();

            foreach (Comment comment in comments)
            {
                var commentUser = await _context.Users.FindAsync(comment.UserId);

                var commentUserModel = new UserModel
                {
                    UserId = commentUser.Id,
                    Username = commentUser.UserName
                };

                var post = await _context.Posts.FindAsync(comment.PostId);

                commentModels.Add(new CommentModel
                {
                    Id = comment.Id,
                    Content = comment.Content,
                    UserModel = commentUserModel,
                    PostId = post.Id,
                    PostName = post.Name
                });
            }

            CommentManagementModel commentManagementModel = new CommentManagementModel
            {
                CommentModels = commentModels
            };

            commentManagementModel.PaginationInfo = new PaginationInfo
            {
                TotalItems = totalItems,
                ItemsPerPage = commentsOnPage,
                CurrentPage = pageIndex,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / commentsOnPage)).ToString()),
                Previous = pageIndex - 1,
                Next = pageIndex + 1
            };

            commentManagementModel.EditCommentModel = new EditCommentModel
            {
                CurrentPage = pageIndex
            };

            if (postId != null)
            {
                commentManagementModel.EditCommentModel.PostId = (int)postId;
            }
            

            return commentManagementModel;
        }

        private async Task<List<ApplicationUser>> GetUserManagementModel_RootUsers()
        {
            return await _context.Users.ToListAsync();
        }

        private async Task<List<ApplicationUser>> GetUserManagementModel_AdminUsers()
        {
            var userList = new List<ApplicationUser>();

            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Root") || await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    continue;
                }
                else
                {
                    userList.Add(user);
                }
            }

            return userList;
        }
        
        private async Task<List<ApplicationUser>> GetUserManagementModel_ModUsers()
        {
            var userList = new List<ApplicationUser>();

            var users = await _context.Users.ToListAsync();

            foreach (var user in users)
            {
                if (await _userManager.IsInRoleAsync(user, "Root") || await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "Moderator"))
                {
                    continue;
                }
                else
                {
                    userList.Add(user);
                }
            }

            return userList;
        }
        
        public async Task<UserManagementModel> GetUserManagementModel(int pageIndex, int usersOnPage, ApplicationUser currentUser)
        {
            List<ApplicationUser> users = new List<ApplicationUser>();

            if (await _userManager.IsInRoleAsync(currentUser, "Root"))
            {
                users = await GetUserManagementModel_RootUsers();
            }
            
            else if (await _userManager.IsInRoleAsync(currentUser, "Admin"))
            {
                users = await GetUserManagementModel_AdminUsers();
            }
            else if (await _userManager.IsInRoleAsync(currentUser, "Moderator"))
            {
                users = await GetUserManagementModel_ModUsers();
            }
            
            int totalItems = users.Count();

            users = users
                .Skip(usersOnPage * (pageIndex - 1))
                .Take(usersOnPage)
                .ToList();

            List<UserModel> userModels = new List<UserModel>();

            foreach (ApplicationUser user in users)
            {
                userModels.Add(new UserModel
                {
                    UserId = user.Id,
                    Username = user.UserName
                });
            }

            UserManagementModel userManagementModel = new UserManagementModel
            {
                UserModels = userModels
            };

            userManagementModel.PaginationInfo = new PaginationInfo
            {
                TotalItems = totalItems,
                ItemsPerPage = usersOnPage,
                CurrentPage = pageIndex,
                TotalPages = int.Parse(Math.Ceiling(((decimal)totalItems / usersOnPage)).ToString()),
                Previous = pageIndex - 1,
                Next = pageIndex + 1
            };

            return userManagementModel;
        }

        public async Task EditComment(EditCommentModel editCommentModel)
        {
            Comment comment = await _context.Comments.FindAsync(editCommentModel.CommentId);
            comment.Content = editCommentModel.Content;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteComment(EditCommentModel editCommentModel)
        {
            Comment comment = await _context.Comments.FindAsync(editCommentModel.CommentId);
            _context.Remove(comment);

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCommentManagement(EditCommentModel editCommentModel)
        {
            Comment comment = await _context.Comments.FindAsync(editCommentModel.CommentId);

            _context.Remove(comment);

            await _context.SaveChangesAsync();
        }

        public async Task DeletePostManagement(EditPostModel editPostModel)
        {
            Post post = await _context.Posts.FindAsync(editPostModel.PostId);

            List<Comment> comments = await _context.Comments.Where(c => c.PostId == post.Id).ToListAsync();

            _context.Remove(post);
            _context.RemoveRange(comments);

            await _context.SaveChangesAsync();
        }

        public async Task<EditUserModel> GetUser(string userId, ApplicationUser currentUser)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userClaims = await _userManager.GetClaimsAsync(user);

            EditUserModel model = new EditUserModel
            {
                UserId = userId,
                UserName = user.UserName
            };

            bool isCurrentUserRoot = await _userManager.IsInRoleAsync(currentUser, "Root");
            bool isCurrentUserRootOrAdmin = await _userManager.IsInRoleAsync(currentUser, "Root") || await _userManager.IsInRoleAsync(currentUser, "Admin");
            bool isEditUserRootOrAdmin = await _userManager.IsInRoleAsync(user, "Root") || await _userManager.IsInRoleAsync(user, "Admin");
            var currentUserClaims = await _userManager.GetClaimsAsync(currentUser);

            if ((isCurrentUserRootOrAdmin && !isEditUserRootOrAdmin) || isCurrentUserRoot)
            {
                foreach (var role in _roleManager.Roles)
                {
                    if (role.Name == "Root" || role.Name == "Admin")
                    {
                        if (!isCurrentUserRoot)
                        {
                            continue;
                        }
                    }

                    var userRolesModel = new UserRolesModel
                    {
                        RoleId = role.Id,
                        RoleName = role.Name
                    };

                    if (await _userManager.IsInRoleAsync(user, role.Name))
                    {
                        userRolesModel.IsSelected = true;
                    }
                    else
                    {
                        userRolesModel.IsSelected = false;
                    }

                    model.Roles.Add(userRolesModel);
                }
            }

            foreach (Subforum subforum in await _context.Subforums.ToListAsync())
            {
                if (currentUserClaims.Any(c => c.Type == "ModAccess_" + subforum.Id.ToString() && c.Value == "true") || isCurrentUserRootOrAdmin)
                {
                    SubforumAccess subforumAccess = new SubforumAccess
                    {
                        SubforumId = subforum.Id,
                        SubforumName = subforum.Name
                    };
           

                    if (userClaims.Any(c => c.Type == "UserAccess_" + subforum.Id.ToString() && c.Value == "true"))
                    {
                        subforumAccess.UserAccess = true;
                    }
                    else
                    {
                        subforumAccess.UserAccess = false;
                    }

                    if ((isCurrentUserRootOrAdmin && !isEditUserRootOrAdmin) || isCurrentUserRoot)
                    {
                        if (userClaims.Any(c => c.Type == "ModAccess_" + subforum.Id.ToString() && c.Value == "true"))
                        {
                            subforumAccess.ModAccess = true;
                        }
                        else
                        {
                            subforumAccess.ModAccess = false;
                        }
                    }

                    model.SubforumAccess.Add(subforumAccess);
                }
            }

            return model;
        }

        public async Task EditUserRoles(EditUserModel editUserModel)
        {
            var user = await _userManager.FindByIdAsync(editUserModel.UserId);
            var roles = await _userManager.GetRolesAsync(user);

            var result = await _userManager.RemoveFromRolesAsync(user, roles);

            result = await _userManager.AddToRolesAsync(user,
                editUserModel.Roles.Where(x => x.IsSelected).Select(y => y.RoleName));
        }

        public async Task AddInitialUserClaims(ApplicationUser user)
        {
            var result = await _userManager.AddClaimsAsync(user,
                _context.Subforums.Select(c => new Claim("UserAccess_" + c.Id.ToString(), "true")));
        }

        public async Task EditUserAccess(EditUserModel editUserModel)
        {
            var user = await _userManager.FindByIdAsync(editUserModel.UserId);
            var claims = await _userManager.GetClaimsAsync(user);

            var result = await _userManager.RemoveClaimsAsync(user, claims.Where(c => c.Type.Substring(0, 11) == "UserAccess_"));

            result = await _userManager.AddClaimsAsync(user,
                editUserModel.SubforumAccess.Select(c => new Claim("UserAccess_" + c.SubforumId.ToString(), c.UserAccess ? "true" : "false")));
        }

        public async Task EditUserModeration(EditUserModel editUserModel)
        {
            var user = await _userManager.FindByIdAsync(editUserModel.UserId);
            var claims = await _userManager.GetClaimsAsync(user);

            var result = await _userManager.RemoveClaimsAsync(user, claims.Where(c => c.Type.Substring(0, 10) == "ModAccess_"));

            result = await _userManager.AddClaimsAsync(user,
                editUserModel.SubforumAccess.Select(c => new Claim("ModAccess_" + c.SubforumId.ToString(), c.ModAccess ? "true" : "false")));
        }
    }
}
