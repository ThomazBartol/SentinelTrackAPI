namespace SentinelTrack.Domain.Entities
{
    public class Moto
    {
        public Guid Id { get; set; }
        public string Plate { get; set; } = default!;
        public string? Model { get; set; }
        public string? Color { get; set; }
        public Guid YardId { get; set; }
        public Yard? Yard { get; set; }
    }
}
