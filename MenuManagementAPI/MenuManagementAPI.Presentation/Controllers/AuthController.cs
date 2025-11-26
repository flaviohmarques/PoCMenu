using MenuManagementAPI.Application.Common;
using MenuManagementAPI.Application.DTOs;
using MenuManagementAPI.Infrastructure.Services;
using Microsoft.AspNetCore.Mvc;

namespace MenuManagementAPI.Presentation.Controllers;

/// <summary>
/// Controller para autenticação
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController(IJwtService jwtService, ILogger<AuthController> logger) : ControllerBase
{
    /// <summary>
    /// Realiza login e retorna um token JWT
    /// </summary>
    /// <param name="request">Credenciais de login</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Token JWT e informações do usuário</returns>
    [HttpPost("login")]
    [ProducesResponseType<ApiResponse<LoginResponse>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<LoginResponse>>(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType<ApiResponse<LoginResponse>>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<LoginResponse>>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Para POC, validação simples (em produção, validar contra banco de dados)
            // TODO: Implementar validação real com repositório de usuários
            if (request.Username == "admin" && request.Password == "admin123")
            {
                var token = await jwtService.GenerateTokenAsync("1", request.Username, ["Admin"], cancellationToken);

                var response = new LoginResponse
                {
                    Token = token,
                    Username = request.Username,
                    ExpiresIn = 3600 // 1 hora
                };

                logger.LogInformation("Login realizado com sucesso para usuário: {Username}", request.Username);

                return Ok(ApiResponse<LoginResponse>.SuccessResponse(
                    response,
                    "Login realizado com sucesso"));
            }

            logger.LogWarning("Tentativa de login falhou para usuário: {Username}", request.Username);

            return Unauthorized(ApiResponse<LoginResponse>.ErrorResponse(
                "Usuário ou senha inválidos"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao realizar login para usuário: {Username}", request.Username);

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<LoginResponse>.ErrorResponse("Erro interno do servidor"));
        }
    }

    /// <summary>
    /// Valida um token JWT
    /// </summary>
    /// <param name="request">Token a ser validado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Indica se o token é válido</returns>
    [HttpPost("validate")]
    [ProducesResponseType<ApiResponse<bool>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ApiResponse<bool>>(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ApiResponse<bool>>> ValidateToken(
        [FromBody] ValidateTokenRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var principal = await jwtService.ValidateTokenAsync(request.Token, cancellationToken);
            var isValid = principal is not null;

            logger.LogInformation("Token validado: {IsValid}", isValid);

            return Ok(ApiResponse<bool>.SuccessResponse(
                isValid,
                isValid ? "Token válido" : "Token inválido"));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao validar token");

            return StatusCode(
                StatusCodes.Status500InternalServerError,
                ApiResponse<bool>.ErrorResponse("Erro interno do servidor"));
        }
    }
}