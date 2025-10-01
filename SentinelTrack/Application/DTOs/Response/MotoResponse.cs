namespace SentinelTrack.Application.DTOs.Response
{
    /// <summary>
    /// Representa os dados públicos de uma moto expostos pela API.
    /// </summary>
    public class MotoResponse
    {
        /// <summary>
        /// Identificador único da moto.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Placa da moto.
        /// </summary>
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
        public Guid YardId { get; set; }
    }
}
