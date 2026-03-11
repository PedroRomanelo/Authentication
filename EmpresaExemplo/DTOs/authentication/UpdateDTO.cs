using EmpresaExemplo.Enums;


namespace EmpresaExemplo.DTOs.authentication;

public record UpdateDTO
(
    string? NomeCompleto,
    States? Estado,
    string? Cidade
);