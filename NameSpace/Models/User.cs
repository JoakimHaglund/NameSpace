using Microsoft.AspNetCore.Identity;

namespace NameSpace.Models
{
    public class User : IdentityUser
    {
        public ICollection<UserReaction>? UserReactions { get; set; }
        public string? PartnerUserId { get; set; }
        public User? PartnerUser { get; set; }

    }
}
