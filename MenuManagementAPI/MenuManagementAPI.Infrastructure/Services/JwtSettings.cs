namespace MenuManagementAPI.Infrastructure.Services;

/// <summary>
/// Configurações para geração de tokens JWT
/// </summary>
public sealed class JwtSettings
{
    public const string SectionName = "JwtSettings";

    /// <summary>
    /// Chave secreta para assinatura do token (mínimo 32 caracteres)
    /// </summary>
    public required string SecretKey { get; init; }

    /// <summary>
    /// Emissor do token
    /// </summary>
    public required string Issuer { get; init; }

    /// <summary>
    /// Audiência do token
    /// </summary>
    public required string Audience { get; init; }

    /// <summary>
    /// Tempo de expiração do token em minutos
    /// </summary>
    public int ExpirationMinutes { get; init; } = 60;
}