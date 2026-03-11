using EmpresaExemplo.Data;
using EmpresaExemplo.DTOs.Orders;
using EmpresaExemplo.Enums;
using EmpresaExemplo.Models.AuthDB;
using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;

namespace EmpresaExemplo.Services;
public class OrderService
{
    private readonly AuthContext _AuthContext;

    public OrderService (AuthContext authContext)
    {
        _AuthContext = authContext; 
    }

    public async Task<List<OrderResponseDTO>> GetPedidosByClienteId(int clienteId)
    {
        return await _AuthContext.Pedidos
            .Where(p => p.ClienteId == clienteId)
            .Select(p => new OrderResponseDTO
            (
                    p.Id,
                    p.ClienteId,
                    p.OrderNumber,
                    p.DiscountPercent,
                    p.PaymentMethod,
                    p.Frete,
                    p.ValorTotal,
                    p.Horario
            ))
            .ToListAsync();
    }

    public async Task<Pedido?> GetPedidoById(int id)
    {
        return await _AuthContext.Pedidos.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<OrderItensResponseDTO> CreateOrderItens( int clienteId, States estadoCliente, OrderDTO request)
    {
        using var transaction = await _AuthContext.Database.BeginTransactionAsync();

        decimal valorFrete = FretePorEstado(estadoCliente);

        if (!Enum.IsDefined(typeof(PaymentMethodEnum), request.PaymentMethod.Value))
        {
            throw new ArgumentException("Método de pagamento inválido. Valores aceitos: 0 (DebitCard), 1 (CreditCard), 2 (Pix), 3 (Boleto).");
        }

        decimal Discount = request.DiscountPercent;

        if(Discount >= 100 || Discount<0)
        {
            throw new Exception("Ta querendo ganhar dinheiro com a compra ?");
        }

        var pedido = new Pedido
        {
            ClienteId = clienteId,
            OrderNumber = request.OrderNumber,
            Platform = request.Platform,
            UsedCoupon = request.UsedCoupon,
            DiscountPercent = Discount,
            PaymentMethod = (int)request.PaymentMethod.Value,
            Frete = valorFrete,
            ValorTotal = 0,
            Horario = DateTime.Now,
        };

        await _AuthContext.Pedidos.AddAsync(pedido);
        await _AuthContext.SaveChangesAsync(); // Gera o Id auto-incrementado do pedido

        var itensCriados = new List<ItensPedido>();
        decimal totalItens = 0;

        foreach (var item in request.Itens) // n+1
        {
            var produto = await _AuthContext.Produtos.FirstOrDefaultAsync(p => p.Id == item.ProductId);

            if (produto == null)
            {
                throw new ArgumentException("Esse produto nem existe");
            }

            var valorTotalItem = (produto.ValorUnitario * item.Quantity)*(1-(pedido.DiscountPercent/100));

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
        pedido.ValorTotal = totalItens + valorFrete;

        await _AuthContext.SaveChangesAsync();

        await transaction.CommitAsync();

        var listaItens = itensCriados.Select(i => new OrderItensDTO
        (
            i.ProdutoId,
            i.Quantidade
        )).ToList();

        return new OrderItensResponseDTO
        (
            pedido.Id,
            request.Platform,
            request.UsedCoupon,
            pedido.Id,
            listaItens
        );
    }

    private decimal FretePorEstado(States estado)
    {
        return estado switch
        {
            States.AC or States.AP or States.AM or States.PA or States.RO or States.RR or States.TO => 8.00m,
            States.AL or States.BA or States.CE or States.MA or States.PB or States.PE or States.PI or States.RN or States.SE => 49.49m,
            States.DF or States.GO or States.MT or States.MS => 5.00m,
            States.PR or States.RS or States.SC => 3.90m,
            States.ES or States.MG or States.RJ or States.SP => 0.00m,
            _ => throw new ArgumentException("Estado inválido para cálculo de frete.")
        };
    }
}