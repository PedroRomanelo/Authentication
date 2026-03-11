namespace EmpresaExemplo.DTOs.Orders;

public record OrderResponseDTO
(
    int Id,
    int ClienteId,
    string NumeroPedido,
    decimal PorcentualDesconto,
    int PaymentMethod,
    decimal Frete,
    decimal ValorTotal,
    DateTime Horario
);