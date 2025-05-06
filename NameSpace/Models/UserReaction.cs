namespace NameSpace.Models
{
    public class UserReaction
    {
        public required string UserId { get; set; }
        public required Guid NameInfoId { get; set; }
        public required ReactionType Reaction { get; set; }

        public User? User { get; set; }
        public NameInfo? NameInfo { get; set; }
    }
    public enum ReactionType
    {
        Dislike = 0,
        Like = 1,
        Favorite = 2,
    }
}
