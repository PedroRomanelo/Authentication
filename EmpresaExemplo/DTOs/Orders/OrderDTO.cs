using EmpresaExemplo.DTOs;
using EmpresaExemplo.Enums;
using System.ComponentModel.DataAnnotations;

namespace EmpresaExemplo.DTOs.Orders;

public record OrderDTO
    (
        [Required(ErrorMessage = "Campo de Platforma do produto é obrigatório")]
        string Platform,

        [Required(ErrorMessage = "O numero do pedido é obrigatório")]
        string OrderNumber,

        [Required(ErrorMessage = "Campo de cupom do produto é obrigatório")] //default 0
        string UsedCoupon,

        [Required(ErrorMessage = "Campo de desconto do produto é obrigatório")]
        decimal DiscountPercent,

        [Required(ErrorMessage = "Campo para informar o método de pagamento é obrigatório")]
        PaymentMethodEnum? PaymentMethod, //enum é value type

        [Required(ErrorMessage = "A lista de itens é obrigatório")]
        List<OrderItensDTO> Itens
    );
