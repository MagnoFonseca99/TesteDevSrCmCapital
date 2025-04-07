using CmCapitalDevSrProject.Models.DTOs;
using CmCapitalDevSrProject.Services;
using CmCapitalDevSrProject.Services.Models.DTOs;
using Microsoft.AspNetCore.Mvc;

namespace CmCapitalDevSrProject.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LoginController : ControllerBase
{
    private readonly AuthService _auth;
    private readonly IConfiguration _config;

    public LoginController(AuthService auth, IConfiguration config)
    {
        _auth = auth;
        _config = config;
    }

    [HttpPost]
    public IActionResult Login([FromBody] LoginDto dto)
    {
        var usuarioEsperado = _config["AuthUsuarioFake:Usuario"];
        var senhaEsperada = _config["AuthUsuarioFake:Senha"];

        if (dto.Email != usuarioEsperado || dto.Senha != senhaEsperada)
            return Unauthorized(new { mensagem = "Credenciais inválidas." });

        var token = _auth.GerarToken(dto.Email);

        return Ok(new TokenRespostaDto
        {
            Token = token,
            ExpiraEm = DateTime.UtcNow.AddMinutes(int.Parse(_config["Jwt:ExpiresInMinutes"]!))
        });
    }
}