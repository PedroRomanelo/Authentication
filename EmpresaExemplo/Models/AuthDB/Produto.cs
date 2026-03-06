using System;
using System.Collections.Generic;

namespace EmpresaExemplo.Models.AuthDB;

public partial class Produto
{
    public int Id { get; set; }

    public string NomeDoProduto { get; set; } = null!;

    public string Sku { get; set; } = null!;

    public decimal ValorUnitario { get; set; }

    public virtual ICollection<ItensPedido> ItensPedidos { get; set; } = new List<ItensPedido>();
}
