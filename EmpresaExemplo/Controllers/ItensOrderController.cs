using EmpresaExemplo.DTOs.Orders;
using EmpresaExemplo.Enums;
using EmpresaExemplo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EmpresaExemplo.Controllers;

[Authorize]
[Route("[Controller]")]
[ApiController]
public class ItensOrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public ItensOrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> AddItens([FromBody] OrderDTO request)
    {

        var clientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var stateClaim = User.FindFirstValue("state");

        if(string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out int clienteId))
        {
            return Unauthorized("Id não encontrado no jwt");
        }

        if (string.IsNullOrEmpty(stateClaim) || !Enum.TryParse<States>(stateClaim, out var estadoCliente))
        {
            return Unauthorized("Estado não encontrado ou inválido no jwt");
        }

        try
        {
            var resultado = await _orderService.CreateOrderItens(clienteId, estadoCliente, request);

            return Ok(resultado);
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid(); //return 403
        }
        catch(ArgumentException ex)
        {
            return BadRequest(ex.Message); 
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetMeusPedidos()
    {
        var clientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out int clienteId))
        {
            return Unauthorized("Id não encontrado no jwt");
        }

        var pedidos = await _orderService.GetPedidosByClienteId(clienteId);

        return Ok(pedidos);
    }
}
