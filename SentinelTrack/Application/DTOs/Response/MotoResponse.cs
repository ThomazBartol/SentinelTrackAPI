namespace SentinelTrack.Application.DTOs.Response
{
    /// <summary>
    /// Representa os dados p�blicos de uma moto expostos pela API.
    /// </summary>
    public class MotoResponse
    {
        /// <summary>
        /// Identificador �nico da moto.
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
        /// Identificador do p�tio do qual est� atrelada.
        /// </summary>
        public Guid YardId { get; set; }
    }
}
