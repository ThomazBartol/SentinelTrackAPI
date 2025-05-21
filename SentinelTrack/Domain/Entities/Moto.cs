namespace SentinelTrack.Domain.Entities
{
    public class Moto
    {
        public Guid Id { get; private set; }

        public string Plate { get; private set; }

        public string? Model { get; private set; }

        public string? Color { get; private set; }

        public int YardId { get; set; }       
       
        public Yard Yard { get; set; }
    }
}
