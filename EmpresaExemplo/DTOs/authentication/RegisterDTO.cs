using System.ComponentModel.DataAnnotations;
using EmpresaExemplo.Enums;

namespace EmpresaExemplo.DTOs.authentication;

public record RegisterDTO
(
    [Required(ErrorMessage = "Campo CompleteName é obrigatório.´Por favor preencha-o.")]
    string CompleteName,

    [Required(ErrorMessage = "Campo Email é obrigatório.´Por favor preencha-o.")]
    [EmailAddress(ErrorMessage = "Por favor preencha o campo com o formato adequado.")]
    string Email,

    [Required(ErrorMessage = "Campo Password é obrigatório.´Por favor preencha-o.")]
    [MinLength(6, ErrorMessage = "O campo senha deve conter ao menos 6 caracteres. Tente Novamente.")]
    string Password,

    [Required(ErrorMessage = "Campo Document é obrigatório.´Por favor preencha-o.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "A quantidade exata de caracteres para o CPF deve ser de 11 digitos, apenas números")]
    string Document,

    [Required(ErrorMessage = "Campo State é obrigatório.´Por favor preencha-o.")]
    States State,

    [Required(ErrorMessage = "Campo City é obrigatório.´Por favor preencha-o.")]
    [StringLength(100, ErrorMessage = "O campo nome não pode exceder 100 caracteres")]
    string City
);