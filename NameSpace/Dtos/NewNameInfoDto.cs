namespace NameSpace.Dtos
{
    public class NewNameInfoDto
    {
        public required string Name { get; set; }
        public int Antal { get; set; }
        public string? DescriptionOfName { get; set; }

        public int? PartnerReaction { get; set; } = null;
        public required int Gender { get; set; }

        // 👇 Detta behövs för att HashSet ska fatta om två objekt är samma 
        public override bool Equals(object? obj)
        {
            if (obj is not NewNameInfoDto other) return false;
            return string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase) && Gender == other.Gender;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name.ToLowerInvariant(), Gender);
        }
    }
}
