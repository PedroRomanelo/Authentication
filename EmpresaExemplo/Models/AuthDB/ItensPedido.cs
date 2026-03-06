using System;
using System.Collections.Generic;

namespace EmpresaExemplo.Models.AuthDB;

public partial class ItensPedido
{
    public int Id { get; set; }

    public int Quantidade { get; set; }

    public int ProdutoId { get; set; }

    public int PedidoId { get; set; }

    public virtual Pedido Pedido { get; set; } = null!;

    public virtual Produto Produto { get; set; } = null!;
}
