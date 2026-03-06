namespace EmpresaExemplo.DTOs.authentication;

using System.ComponentModel.DataAnnotations;

public class RegisterDTO
{
    [Required(ErrorMessage = "Campo CompleteName é obrigatório.´Por favor preencha-o.")]
    public string CompleteName { get; set; }

    [Required(ErrorMessage = "Campo Email é obrigatório.´Por favor preencha-o.")]
    [EmailAddress(ErrorMessage = "Por favor preencha o campo com o formato adequado.")]

    public string Email { get; set; }

    [Required(ErrorMessage = "Campo Password é obrigatório.´Por favor preencha-o.")]
    [MinLength(6, ErrorMessage = "O campo senha deve conter ao menos 6 caracteres. Tente Novamente.")]

    public string Password { get; set; }

    [Required(ErrorMessage = "Campo Document é obrigatório.´Por favor preencha-o.")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "A quantidade exata de caracteres para o CPF deve ser de 11 digitos, apenas números")]


    public string Document {  get; set; }

    [Required(ErrorMessage = "Campo State é obrigatório.´Por favor preencha-o.")]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Coloque apenas a UF, 2 caracteres.")]
    public string State { get; set; }

    [Required(ErrorMessage = "Campo City é obrigatório.´Por favor preencha-o.")]
    [StringLength(100, ErrorMessage = "O campo nome não pode exceder 100 caracteres")]
    public string City { get; set; }
}