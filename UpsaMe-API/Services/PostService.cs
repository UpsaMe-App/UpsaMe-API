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

        /// <summary>
        /// Feed paginado. Filtra opcionalmente por rol y excluye posts Deleted.
        /// </summary>
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

            var items = await query
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
                    Author = p.User != null ? $"{p.User.FirstName} {p.User.LastName}" : "Anónimo",
                    Subject = p.Subject != null ? p.Subject.Name : null,
                    RepliesCount = p.Replies!.Count
                })
                .ToListAsync<object>();

            return items;
        }

        /// <summary>
        /// Crea un post validando campos básicos por rol.
        /// </summary>
        public async Task<Post> CreateAsync(Post post)
        {
            if (post == null) throw new ArgumentNullException(nameof(post));
            if (string.IsNullOrWhiteSpace(post.Content))
                throw new InvalidOperationException("El contenido no puede estar vacío.");

            // Normaliza
            post.Id = post.Id == Guid.Empty ? Guid.NewGuid() : post.Id;
            post.CreatedAtUtc = DateTime.UtcNow;
            post.UpdatedAtUtc = null;
            post.Status = PostStatus.Active;

            // Reglas por rol
            switch (post.Role)
            {
                case PostRole.Helper:
                    // Para ayudante, si declara cupos debe ser >= 1
                    if (post.Capacity.HasValue && post.Capacity.Value < 1)
                        throw new InvalidOperationException("Capacity debe ser >= 1 para posts de Ayudante.");
                    post.CapacityUsed = Math.Max(0, post.CapacityUsed);
                    break;

                case PostRole.Student:
                    // Student puede tener Subject opcional, sin reglas extra por ahora
                    break;

                case PostRole.Comment:
                    // Comment ignora Capacity/CapacityUsed
                    post.Capacity = null;
                    post.CapacityUsed = 0;
                    break;
            }

            // Si trae SubjectId, verifica que exista
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

        /// <summary>
        /// Agrega una respuesta a un post (si no está eliminado).
        /// </summary>
        public async Task<PostReply> AddReplyAsync(Guid postId, PostReply reply)
        {
            if (reply == null) throw new ArgumentNullException(nameof(reply));
            if (string.IsNullOrWhiteSpace(reply.Content))
                throw new InvalidOperationException("El contenido de la respuesta no puede estar vacío.");

            var post = await _context.Posts
                .Where(p => p.Id == postId && p.Status != PostStatus.Deleted)
                .FirstOrDefaultAsync();

            if (post == null)
                throw new InvalidOperationException("El post no existe o fue eliminado.");

            reply.Id = reply.Id == Guid.Empty ? Guid.NewGuid() : reply.Id;
            reply.PostId = postId;
            reply.CreatedAtUtc = DateTime.UtcNow;

            _context.PostReplies.Add(reply);
            await _context.SaveChangesAsync();

            return reply;
        }
    }
}
