using EmpresaExemplo.Data;
using EmpresaExemplo.Models.AuthDB;
using Microsoft.EntityFrameworkCore;
using EmpresaExemplo.DTOs.Orders;

namespace EmpresaExemplo.Services;
public class OrderService
{
    private readonly AuthContext _AuthContext;

    public OrderService (AuthContext authContext)
    {
        _AuthContext = authContext; 
    }

    public async Task<List<Pedido>> GetPedidosByClienteId(int clienteId)
    {
        return await _AuthContext.Pedidos
            .Where(p => p.ClienteId == clienteId)
            .ToListAsync();
    }

    public async Task<Pedido?> GetPedidoById(int id)
    {
        return await _AuthContext.Pedidos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<List<OrderItensResponseDTO>> CreateOrderItens( int clienteId, List<OrderItensDTO> request)
    {
        using var transaction = await _AuthContext.Database.BeginTransactionAsync();

        var pedido = new Pedido
        {
            ClienteId = clienteId,
            ValorTotal = 0,
            Horario = DateTime.Now
        };

        await _AuthContext.Pedidos.AddAsync(pedido);
        await _AuthContext.SaveChangesAsync(); // Gera o Id auto-incrementado do pedido

        var itensCriados = new List<ItensPedido>();
        decimal totalItens = 0;

        foreach (var item in request) // n+1
        {
            var produto = await _AuthContext.Produtos.FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (produto == null)
            {
                throw new ArgumentException("Esse produto nem existe");
            }

            var valorTotalItem = (produto.ValorUnitario * item.Quantity);

            var novoItem = new ItensPedido
            {
                PedidoId = pedido.Id,
                ProdutoId = produto.Id,
                Quantidade = item.Quantity
            };

            totalItens += valorTotalItem;

            itensCriados.Add(novoItem);
            await _AuthContext.ItensPedidos.AddAsync(novoItem);
        }
        pedido.ValorTotal = totalItens;

        await _AuthContext.SaveChangesAsync();

        await transaction.CommitAsync();

        return itensCriados.Select(i => new OrderItensResponseDTO
        {
            Id = i.Id,
            PedidoId = i.PedidoId,
            ProdutoId = i.ProdutoId,
            Quantidade = i.Quantidade,
        }).ToList();
    }
}
