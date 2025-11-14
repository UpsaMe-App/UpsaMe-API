using UpsaMe_API.DTOs.User;

namespace UpsaMe_API.Helpers
{
    public static class AvatarCatalog
    {
        private static readonly List<AvatarOptionDto> _avatars = new()
        {
            new AvatarOptionDto { Id = "baby", Url = "/avatars/baby.png", Label = "Baby" },
            new AvatarOptionDto { Id = "carita-feliz-roja", Url = "/avatars/carita-feliz-roja.png", Label = "Carita roja" },
            new AvatarOptionDto { Id = "chicken", Url = "/avatars/chicken.png", Label = "Chicken" },
            new AvatarOptionDto { Id = "panda-feliz", Url = "/avatars/panda-feliz.png", Label = "Panda feliz" },
            new AvatarOptionDto { Id = "purpple-penguin", Url = "/avatars/purpple-penguin.png", Label = "Pingüino púrpura" },
            new AvatarOptionDto { Id = "superhero", Url = "/avatars/superhero.png", Label = "Superhero" },
            new AvatarOptionDto { Id = "superhero-girl", Url = "/avatars/superhero-girl.png", Label = "Superhero Girl" },
            new AvatarOptionDto { Id = "un-ojo", Url = "/avatars/un-ojo.png", Label = "Un ojo" }
        };

        public static IReadOnlyList<AvatarOptionDto> GetAll() => _avatars;

        public static string? ResolveUrl(string avatarId)
        {
            return _avatars
                .FirstOrDefault(a => 
                    a.Id.Equals(avatarId, StringComparison.OrdinalIgnoreCase))
                ?.Url;
        }
    }
}