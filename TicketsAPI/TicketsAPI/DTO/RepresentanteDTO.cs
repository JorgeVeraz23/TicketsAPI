namespace TicketsAPI.DTO
{
    public record RepresentanteCreateDto(
    string Nombres,
    string Apellidos,
    string TipoDocumento,
    string NumeroDocumento,
    string? Telefono,
    string? Email,
    string? Direccion
);

    public record RepresentanteUpdateDto(
        string Nombres,
        string Apellidos,
        string TipoDocumento,
        string NumeroDocumento,
        string? Telefono,
        string? Email,
        string? Direccion
    );

}
