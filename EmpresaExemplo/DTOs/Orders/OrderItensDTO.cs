using System.ComponentModel.DataAnnotations;

namespace EmpresaExemplo.DTOs.Orders;

public record OrderItensDTO
(
    [Required(ErrorMessage = "Campo de Id do produto é obrigatório")]
    int ProductId,

    [Required(ErrorMessage = "Campo de quantidade do produto é obrigatório")]
    int Quantity
);
