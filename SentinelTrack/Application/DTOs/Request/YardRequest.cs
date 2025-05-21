using System.ComponentModel.DataAnnotations;

namespace SentinelTrack.Application.DTOs.Request
{
    public class YardRequest
    {
        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public int Capacity { get; set; }
    }
}
