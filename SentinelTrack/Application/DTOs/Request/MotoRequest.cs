namespace SentinelTrack.Application.DTOs.Request
{
    public class MotoRequest
    {
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Cor { get; set; }
        public DateTime DataEntrada { get; set; }
        public Guid YardId { get; set; }
    }
}
