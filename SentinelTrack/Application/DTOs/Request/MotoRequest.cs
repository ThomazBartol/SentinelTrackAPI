namespace SentinelTrack.Application.DTOs.Request
{
    public class MotoRequest
    {
        public string Plate { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public Guid YardId { get; set; }
    }
}
