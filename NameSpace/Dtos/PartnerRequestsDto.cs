namespace NameSpace.Dtos
{
    public class PartnerRequestsDto
    {
        public string? CurrentPartner { get; set; }
        public List<PartnerRequestSimple>? MyPartnerRequests { get; set; }
        public List<PartnerRequestSimple>? PartnerRequests { get; set; }

    }

    public class PartnerRequestSimple
    {
        public Guid Id { get; set; }
        public required string UserId { get; set; }
    }

}
