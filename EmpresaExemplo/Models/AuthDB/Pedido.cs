using System;
using System.Collections.Generic;

namespace EmpresaExemplo.Models.AuthDB;

public partial class Pedido
{
    public int Id { get; set; }

    public int ClienteId { get; set; }

    public decimal Frete { get; set; }

    public decimal ValorTotal { get; set; }

    public DateTime Horario { get; set; }

    public virtual User Cliente { get; set; } = null!;

    public virtual ICollection<ItensPedido> ItensPedidos { get; set; } = new List<ItensPedido>();
}
