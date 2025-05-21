using System.ComponentModel.DataAnnotations;

namespace SentinelTrack.Application.DTOs.Request
{
    public class MotoRequest
    {
        [Required(ErrorMessage = "A placa é obrigatória.")]
        public string Plate { get; set; }
        public string Model { get; set; }
        public string Color { get; set; }
        public Guid YardId { get; set; }
    }
}
