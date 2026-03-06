using EmpresaExemplo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using EmpresaExemplo.DTOs.Orders;


namespace EmpresaExemplo.Controllers;

[Authorize]
[Route("itensorder")]
[ApiController]
public class ItensOrderController : ControllerBase
{
    private readonly OrderService _orderService;

    public ItensOrderController(OrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> AddItens([FromBody] List<OrderItensDTO> request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var clientIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if(string.IsNullOrEmpty(clientIdClaim) || !int.TryParse(clientIdClaim, out int clienteId))
        {
            return Unauthorized("Id não encontrado no jwt");
        }

        try
        {
            var resultado = await _orderService.CreateOrderItens(clienteId, request);

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

    [HttpGet("orders")]
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
