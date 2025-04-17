using Microsoft.AspNetCore.Identity;

namespace NameSpace.Models
{
    public class User : IdentityUser
    {
        public ICollection<UserReaction>? UserReactions { get; set; }

    }
}
