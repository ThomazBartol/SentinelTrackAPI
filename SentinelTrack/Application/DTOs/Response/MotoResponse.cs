namespace SentinelTrack.Application.DTOs.Response
{
    public class MotoResponse
    {
        public Guid Id { get; set; }
        public string Placa { get; set; }
        public string Modelo { get; set; }
        public string Cor { get; set; }
        public DateTime DataEntrada { get; set; }
        public Guid YardId { get; set; }
    }
}
