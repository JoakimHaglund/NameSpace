namespace NameSpace.Models
{
    public class ConfirmPartner
    {
        public required Guid Id { get; set; }
        public required Guid Token { get; set; }
        public required string RequestingUserId { get; set; }
        public required string RecivingUserId { get; set; }
    }
}
