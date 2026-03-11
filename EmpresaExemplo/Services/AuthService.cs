using EmpresaExemplo.DTOs.authentication;
using EmpresaExemplo.Data;
using Microsoft.EntityFrameworkCore;

namespace EmpresaExemplo.Services;
public class AuthService
{
    private readonly AuthContext _AuthContext;

    public AuthService ( AuthContext authContext)
    {
        _AuthContext = authContext; 
    }
    public async Task UpdateCliente(int id, UpdateDTO request)
    {
        var cliente = await _AuthContext.Clientes.FirstOrDefaultAsync(c => c.Id == id);

        if (cliente == null)
        {
            throw new ArgumentException("Cliente não encontrado.");
        }

        cliente.NomeCompleto = request.NomeCompleto ?? cliente.NomeCompleto;
        cliente.Estado = request.Estado?.ToString() ?? cliente.Estado;
        cliente.Cidade = request.Cidade ?? cliente.Cidade;

        await _AuthContext.SaveChangesAsync();
    }
};
