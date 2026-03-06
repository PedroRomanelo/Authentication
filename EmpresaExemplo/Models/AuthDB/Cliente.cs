using System;
using System.Collections.Generic;

namespace EmpresaExemplo.Models.AuthDB;

public partial class Cliente
{
    public string NomeCompleto { get; set; } = null!;

    public string Documento { get; set; } = null!;

    public string Estado { get; set; } = null!;

    public string Cidade { get; set; } = null!;

    public int Id { get; set; }

    public virtual User IdNavigation { get; set; } = null!;
}
