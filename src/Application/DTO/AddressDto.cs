namespace Application.DTO
{
    /// <summary>
    /// Contrato de retorno da porta <see cref="Application.Interfaces.IExampleService"/>.
    /// Mantém a camada de Aplicação independente do formato de DTO do provedor externo.
    /// </summary>
    public class AddressDto
    {
        public string ZipCode { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string Complement { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
    }
}
