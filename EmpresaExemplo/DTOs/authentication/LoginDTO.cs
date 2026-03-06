using System.ComponentModel.DataAnnotations;


namespace EmpresaExemplo.DTOs.authentication;

public class LoginDTO
{
    [Required(ErrorMessage = "O campo endereço de E-mail é obrigatório. Preencha-o")]
    [EmailAddress(ErrorMessage = "O formato do campo de e-mail está inválido. Tente novamente.")]
    public string Email { get; set; }

    [Required(ErrorMessage = " O campo senha é obrigatório.")]
    public string Password { get; set; }
}