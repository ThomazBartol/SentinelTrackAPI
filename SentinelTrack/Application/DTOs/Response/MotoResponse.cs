namespace SentinelTrack.Application.DTOs.Response
{
    public class MotoResponse
    {
        public Guid Id { get; set; }
        public string Plate { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public Guid YardId { get; set; }
    }
}
