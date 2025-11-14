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

        // ============================================================
        // 📌 1. FEED GENERAL (Home)
        // ============================================================
        public async Task<List<object>> GetFeedAsync(PostRole? role = null, int page = 1, int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var query = _context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Subject)
                .Include(p => p.Replies)
                .Where(p => p.Status != PostStatus.Deleted)
                .AsQueryable();

            if (role.HasValue)
                query = query.Where(p => p.Role == role.Value);

            var rows = await query
                .OrderByDescending(p => p.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Role,
                    p.Status,
                    p.Title,
                    p.Content,
                    p.Capacity,
                    p.CapacityUsed,
                    p.CreatedAtUtc,
                    AuthorId = p.UserId, 
                    Author = p.User != null ? $"{p.User.FirstName} {p.User.LastName}" : "Anónimo",
                    Subject = p.Subject != null ? p.Subject.Name : null,
                    RepliesCount = p.Replies != null ? p.Replies.Count : 0
                })
                .ToListAsync();

            return rows.Cast<object>().ToList();
        }

        // ============================================================
        // 📌 2. CREAR POST
        // ============================================================
        public async Task<Post> CreateAsync(Post post)
        {
            if (post == null)
                throw new ArgumentNullException(nameof(post));

            if (string.IsNullOrWhiteSpace(post.Content))
                throw new InvalidOperationException("El contenido no puede estar vacío.");

            // Normalización
            post.Id = post.Id == Guid.Empty ? Guid.NewGuid() : post.Id;
            post.CreatedAtUtc = DateTime.UtcNow;
            post.UpdatedAtUtc = null;
            post.Status = PostStatus.Active;
            post.CapacityUsed = Math.Max(0, post.CapacityUsed);

            // Reglas por rol
            switch (post.Role)
            {
                case PostRole.Helper:
                    if (post.Capacity.HasValue && post.Capacity.Value < 1)
                        throw new InvalidOperationException("Capacity debe ser >= 1 para posts de Ayudante.");
                    if (!post.SubjectId.HasValue)
                        throw new InvalidOperationException("SubjectId es obligatorio para posts de Ayudante.");
                    break;

                case PostRole.Student:
                    if (!post.SubjectId.HasValue)
                        throw new InvalidOperationException("SubjectId es obligatorio para posts de Estudiante.");
                    if (!post.Capacity.HasValue || post.Capacity.Value < 1)
                        throw new InvalidOperationException("Capacity debe ser >= 1 para posts de Estudiante.");
                    break;

                case PostRole.Comment:
                    post.Capacity = null;
                    post.CapacityUsed = 0;
                    post.SubjectId = null;
                    break;

                default:
                    throw new InvalidOperationException("Role inválido.");
            }

            // Verificar SUBJECT existe
            if (post.SubjectId.HasValue)
            {
                var exists = await _context.Subjects
                    .AsNoTracking()
                    .AnyAsync(s => s.Id == post.SubjectId.Value);

                if (!exists)
                    throw new InvalidOperationException("La materia (SubjectId) no existe.");
            }

            _context.Posts.Add(post);
            await _context.SaveChangesAsync();
            return post;
        }

        // ============================================================
        // 📌 3. REPLY A UN POST
        // ============================================================
        public async Task<PostReply?> AddReplyAsync(Guid postId, PostReply reply)
        {
            if (reply == null)
                throw new ArgumentNullException(nameof(reply));

            if (string.IsNullOrWhiteSpace(reply.Content))
                throw new InvalidOperationException("El contenido de la respuesta no puede estar vacío.");

            var post = await _context.Posts
                .Where(p => p.Id == postId && p.Status != PostStatus.Deleted)
                .FirstOrDefaultAsync();

            if (post == null)
                return null;

            reply.Id = reply.Id == Guid.Empty ? Guid.NewGuid() : reply.Id;
            reply.PostId = postId;
            reply.CreatedAtUtc = DateTime.UtcNow;

            _context.PostReplies.Add(reply);
            await _context.SaveChangesAsync();

            return reply;
        }

        // ============================================================
        // 📌 4. BUSCAR POSTS POR MATERIA (search del frontend)
        // ============================================================
        public async Task<List<object>> SearchPostsBySubjectAsync(string query, int page = 1, int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return new List<object>();

            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;

            var q = query.Trim().ToLower();

            // 1) Buscar materias relacionadas
            var subjectIds = await _context.Subjects
                .AsNoTracking()
                .Where(s =>
                    s.Name.ToLower().Contains(q) ||
                    (s.Slug != null && s.Slug.ToLower().Contains(q)) ||
                    (s.Code != null && s.Code.ToLower().Contains(q)))
                .Select(s => s.Id)
                .ToListAsync();

            if (!subjectIds.Any())
                return new List<object>();

            // 2) Buscar posts relacionados a esas materias
            var queryPosts = _context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Subject)
                .Where(p =>
                    p.Status != PostStatus.Deleted &&
                    p.SubjectId.HasValue &&
                    subjectIds.Contains(p.SubjectId.Value));

            var rows = await queryPosts
                .OrderByDescending(p => p.CreatedAtUtc)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Role,
                    p.Status,
                    p.Title,
                    p.Content,
                    p.Capacity,
                    p.CapacityUsed,
                    p.CreatedAtUtc,
                    AuthorId = p.UserId,
                    Author = p.User != null ? $"{p.User.FirstName} {p.User.LastName}" : "Anónimo",
                    SubjectId = p.SubjectId,
                    Subject = p.Subject != null ? p.Subject.Name : null
                })
                .ToListAsync();

            return rows.Cast<object>().ToList();
        }
    }
}

