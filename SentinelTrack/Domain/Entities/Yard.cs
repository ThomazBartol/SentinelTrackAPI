namespace SentinelTrack.Domain.Entities
{
    public class Yard
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string? Address { get; set; }
        public string? PhoneNumber { get; set; }
        public int? Capacity { get; set; }
        public ICollection<Moto> Motos { get; set; } = new List<Moto>();
    }
}
