namespace SentinelTrack.Domain.Entities
{
    public class Moto
    {
        public Guid Id { get; set; }

        public string Plate { get; set; }

        public string? Model { get; set; }

        public string? Color { get; set; }

        public int YardId { get; set; }       
       
        public Yard Yard { get; set; }
    }
}
