using System.ComponentModel.DataAnnotations;

namespace SentinelTrack.Application.DTOs.Request
{
    public class UserRequest
    {
        [Required]
        public string Name { get; set; } = default!;
        [Required, EmailAddress]
        public string Email { get; set; } = default!;
        public string Role { get; set; } = "user";
    }
}
