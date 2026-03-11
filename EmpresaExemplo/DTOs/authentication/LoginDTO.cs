using System.ComponentModel.DataAnnotations;


namespace EmpresaExemplo.DTOs.authentication;

public record LoginDTO(
    [Required(ErrorMessage = "O campo endereço de E-mail é obrigatório. Preencha-o")]
    [EmailAddress(ErrorMessage = "O formato do campo de e-mail está inválido. Tente novamente.")]
    string Email,

    [Required(ErrorMessage = " O campo senha é obrigatório.")]
    string Password
);