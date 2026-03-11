using Microsoft.EntityFrameworkCore;
using EmpresaExemplo.Data;
using EmpresaExemplo.DTOs;
using EmpresaExemplo.Enums;
using EmpresaExemplo.Models.AuthDB;
using EmpresaExemplo.Models;
using EmpresaExemplo.Services;
using Microsoft.AspNetCore.Mvc;
using EmpresaExemplo.DTOs.authentication;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace EmpresaExemplo.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase //nativo, re. http, status code
{
    private readonly TokenService _tokenService;
    private readonly AuthContext _authContext;
    private readonly AuthService _authService;

    public AuthController(TokenService tokenService, AuthService authService, AuthContext authContext)
    {
        _tokenService = tokenService;
        _authContext = authContext;
        _authService = authService;
    }

    [HttpPost("register")]
    //dto é a variavel local que armazena na memória o json que chega do insomnia
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto) //IActionResult retorna os status code padronizados
    {
        if (!Enum.IsDefined(typeof(States), dto.State))
        {
            return BadRequest(new { message = "Estado fornecido não está correto" });
        }

        if (await _authContext.Users.AnyAsync(u => u.Email == dto.Email))
        {
            return BadRequest(new { message = "E-mail já cadastrado no db" });
        }

        if(await _authContext.Clientes.AnyAsync( c => c.Documento == dto.Document))
        {
            return BadRequest(new { message = "Documento já utilizado anteriormente" });
        }

        using var transaction = await _authContext.Database.BeginTransactionAsync();  // assegura que os dados vão ser salvos em Clientes E Users

        try
        {
            var user = new User
            {
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                CreatedAt = DateTime.UtcNow
            };
            _authContext.Users.Add(user);
            await _authContext.SaveChangesAsync();

            

            var cliente = new Cliente //error toda entidade no EF precisa ter uma PK -> coloquei no id
            {
                Id = user.Id,
                NomeCompleto = dto.CompleteName,
                Documento = dto.Document,
                Estado = dto.State.ToString(),
                Cidade = dto.City
            };
            _authContext.Clientes.Add(cliente);
            await _authContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return Ok(new { message = "Usuário registrado com sucesso." });

        }
        catch (Exception ex) {
            await transaction.RollbackAsync();

            return StatusCode(500, new { message = " Erro ao registrar o cliente", error = ex.ToString() });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO dto)
    {
        var user = await _authContext.Users.FirstOrDefaultAsync( U => U.Email == dto.Email );  //email existe no db ?

        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) //Valida se o usuário existe e se a senha bate com o Hash do banco
        {
            return Unauthorized(new { message = "Credenciais invalida ! " });
        }
        var cliente = await _authContext.Clientes.FirstOrDefaultAsync(c => c.Id == user.Id);
        if(cliente == null)
        {
            return StatusCode(500, new { message = "Cliente não encontrado" });
        }

        States stateEnum = Enum.Parse<States>(cliente.Estado);

        var token = _tokenService.GenerateToken(user.Id, user.Email, stateEnum);

        return Ok(new { token });
    }

    [Authorize]
    [HttpPatch("Patch")]
    public async Task<IActionResult> Update([FromBody] UpdateDTO dto)
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if(string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int clienteId))
        {
            return Unauthorized(new { message = "Token inválido ou id não encontrado" });
        }

        if (dto.Estado.HasValue && !Enum.IsDefined(typeof(States), dto.Estado.Value))
        {
            return BadRequest(new { message = "Estado fornecido é inválido." });
        }

        try
        {
            await _authService.UpdateCliente(clienteId, dto);

            return Ok(new { message = "Cadastro Atualizado com success !" });
        }
        catch ( ArgumentException ex ) //ordem dos catch importa
        {
            return NotFound (new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "erro interno ao atualizar os dados" });
        }
    }
}
