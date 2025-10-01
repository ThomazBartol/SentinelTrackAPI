using System.ComponentModel.DataAnnotations;

namespace SentinelTrack.Application.DTOs.Request
{
    /// <summary>
    /// Representa o payload para criar ou atualizar uma moto.
    /// </summary>
    public class MotoRequest
    {
        /// <summary>
        /// Placa da moto.
        /// </summary>
        [Required]
        public string Plate { get; set; } = default!;

        /// <summary>
        /// Modelo da moto.
        /// </summary>
        public string? Model { get; set; }

        /// <summary>
        /// Cor da moto.
        /// </summary>
        public string? Color { get; set; }

        /// <summary>
        /// Identificador do pátio do qual está atrelada.
        /// </summary>
        [Required]
        public Guid YardId { get; set; }
    }
}
