namespace EmpresaExemplo.DTOs.Orders;

public record OrderItensResponseDTO
(
    int Id,
    string Plataforma,
    string CupomUsado,
    int PedidoId,
    List<OrderItensDTO> Itens
);