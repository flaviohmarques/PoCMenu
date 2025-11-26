namespace MenuManagementAPI.Application.DTOs;
/// <summary>
/// Request para validação de token
/// </summary>
public sealed record ValidateTokenRequest
{
    /// <summary>
    /// Token JWT a ser validado
    /// </summary>
    public required string Token { get; init; }
}