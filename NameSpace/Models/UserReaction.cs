namespace NameSpace.Models
{
    public class UserReaction
    {
        public required string UserId { get; set; }
        public required Guid NameInfoId { get; set; }
        public required ReactionType Reaction { get; set; }
        public bool IsAFavorite { get; set; } = false;

        public required User User { get; set; }
        public required NameInfo NameInfo { get; set; }
    }
    public enum ReactionType
    {
        Like = 1,
        Dislike = 2,
    }
}
