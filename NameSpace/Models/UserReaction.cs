namespace NameSpace.Models
{
    public class UserReaction
    {
        public required string UserId { get; set; }
        public required Guid NameInfoId { get; set; }
        public required ReactionType Reaction { get; set; }
        public bool IsAFavorite { get; set; } = false;

        public User? User { get; set; }
        public NameInfo? NameInfo { get; set; }
    }
    public enum ReactionType
    {
        Like = 1,
        Dislike = 0,
    }
}
