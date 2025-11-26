namespace MenuManagementAPI.Application.DTOs;

/// <summary>
/// Response de login
/// </summary>
public sealed record LoginResponse
{
    /// <summary>
    /// Token JWT gerado
    /// </summary>
    public required string Token { get; init; }

    /// <summary>
    /// Nome do usuário autenticado
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Tempo de expiração do token em segundos
    /// </summary>
    public required int ExpiresIn { get; init; }
}