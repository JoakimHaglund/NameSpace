namespace NameSpace.Models
{
    public class PartnerRequest
    {
        public required Guid Id { get; set; }
        public required string RequestingUserId { get; set; }
        public required string RecivingUserId { get; set; }
    }
}
