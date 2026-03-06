namespace EmpresaExemplo.DTOs.Orders;

public class OrderItensResponseDTO
{
    public int Id { get; set; }
    public int PedidoId { get; set; }
    public int ProdutoId { get; set; }
    public int Quantidade { get; set; }
    public decimal ValorTotal { get; set; }
}