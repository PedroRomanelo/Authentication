using Microsoft.EntityFrameworkCore;
using EmpresaExemplo.Data;
using EmpresaExemplo.DTOs;
using EmpresaExemplo.Enums;
using EmpresaExemplo.Models.AuthDB;
using EmpresaExemplo.Models;
using EmpresaExemplo.Services;
using Microsoft.AspNetCore.Mvc;
using EmpresaExemplo.DTOs.authentication;


namespace EmpresaExemplo.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase //nativo, re. http, status code
{
    private readonly TokenService _tokenService;
    private readonly AuthContext _authContext;

    public AuthController(TokenService tokenService, AuthContext authContext)
    {
        _tokenService = tokenService;
        _authContext = authContext;
    }

    [HttpPost("register")]
    //dto é a variavel local que armazena na memória o json que chega do insomnia
    public async Task<IActionResult> Register([FromBody] RegisterDTO dto) //IActionResult retorna os status code padronizados
    {

        if (!ModelState.IsValid) //validar automaticamente os dados recebido no dto
        {
            return BadRequest(ModelState);
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
                Estado = dto.State,
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

        States stateEnum = Enum.Parse<States>(cliente.Estado); //

        var token = _tokenService.GenerateToken(user.Id, user.Email, stateEnum);

        return Ok(new { token });
    }
}
