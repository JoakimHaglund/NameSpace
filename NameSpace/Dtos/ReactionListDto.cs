namespace NameSpace.Dtos
{
    public class ReactionListDto
    {
        public Guid NameInfoId { get; set; }
        public required string Name { get; set; }
        public int Reaction { get; set; }
        public int? PartnerReaction { get; set; }
        public int Count { get; set; }
        public int Gender { get; set; }
    }
}
