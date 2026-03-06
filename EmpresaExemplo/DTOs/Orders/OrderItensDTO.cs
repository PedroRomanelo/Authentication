using System.ComponentModel.DataAnnotations;

namespace EmpresaExemplo.DTOs.Orders;

public class OrderItensDTO
{
    [Required(ErrorMessage = "Campo de Id do produto é obrigatório")]
    public int ProductId { get; set; }

    [Required(ErrorMessage = "Campo de quantidade do produto é obrigatório")]
    public int Quantity {  get; set; }
}
