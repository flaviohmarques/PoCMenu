namespace MenuManagementAPI.Application.DTOs;

/// <summary>
/// Request para login
/// </summary>
public sealed record LoginRequest
{
    /// <summary>
    /// Nome de usuário
    /// </summary>
    public required string Username { get; init; }

    /// <summary>
    /// Senha do usuário
    /// </summary>
    public required string Password { get; init; }
}
