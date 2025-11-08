using Microsoft.EntityFrameworkCore;
using UpsaMe_API.Data;
using UpsaMe_API.Models;

namespace UpsaMe_API.Services
{
    public class PostService
    {
        private readonly UpsaMeDbContext _context;

        public PostService(UpsaMeDbContext context)
        {
            _context = context;
        }

        public List<object> GetFeedAsync(PostRole? role = null, int page = 1, int pageSize = 10)
        {
            var query = _context.Posts
                .Include(p => p.User)
                .Include(p => p.Subject)
                .Include(p => p.Replies)
                .AsQueryable();

            if (role.HasValue)
                query = query.Where(p => p.Role == role.Value);

            return query
                .OrderByDescending(p => p.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AsEnumerable()
                .Select(p => new
                {
                    p.Id,
                    p.Role,
                    p.Status,
                    p.Content,
                    p.Capacity,
                    p.CapacityUsed,
                    p.CreatedAtUtc,
                    Author = p.User != null
                        ? $"{p.User.FirstName} {p.User.LastName}"
                        : "Anónimo",
                    Subject = p.Subject != null ? p.Subject.Name : null,
                    RepliesCount = p.Replies?.Count ?? 0
                })
                .ToList<object>();
        }

        public async Task<Post> CreateAsync(Post post)
        {
            post.CreatedAtUtc = DateTime.UtcNow;
            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        public async Task<PostReply> AddReplyAsync(Guid postId, PostReply reply)
        {
            reply.PostId = postId;
            reply.CreatedAtUtc = DateTime.UtcNow;
            _context.PostReplies.Add(reply);
            await _context.SaveChangesAsync();
            return reply;
        }
    }
}
