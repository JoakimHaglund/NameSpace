namespace NameSpace.Models
{
    public class NameInfo
    {
        public required Guid Id { get; set; }
        public required string Name { get; set; }
        public int Antal { get; set; }
        public string? DescriptionOfName { get; set; }
        public required Gender Gender { get; set; }
}
    public enum Gender
    {
        Girl = 0,
        Boy = 1,
    }
}
