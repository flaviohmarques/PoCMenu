using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MenuManagementAPI.Infrastructure.Services;

/// <summary>
/// Interface para serviço de geração e validação de tokens JWT
/// </summary>
public interface IJwtService
{
    /// <summary>
    /// Gera um token JWT de forma assíncrona
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="username">Nome do usuário</param>
    /// <param name="roles">Roles/perfis do usuário</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Token JWT gerado</returns>
    Task<string> GenerateTokenAsync(
        string userId,
        string username,
        IEnumerable<string>? roles = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Valida um token JWT de forma assíncrona
    /// </summary>
    /// <param name="token">Token JWT a ser validado</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>ClaimsPrincipal se válido, null caso contrário</returns>
    Task<ClaimsPrincipal?> ValidateTokenAsync(
        string token,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Extrai o ID do usuário de um token
    /// </summary>
    /// <param name="token">Token JWT</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>ID do usuário ou null</returns>
    Task<string?> GetUserIdFromTokenAsync(
        string token,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Serviço para geração e validação de tokens JWT
/// </summary>
public sealed class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<JwtService> _logger;
    private readonly TokenValidationParameters _validationParameters;
    private readonly SigningCredentials _signingCredentials;

    public JwtService(
        IOptions<JwtSettings> jwtSettings,
        ILogger<JwtService> logger)
    {
        ArgumentNullException.ThrowIfNull(jwtSettings);
        ArgumentNullException.ThrowIfNull(logger);

        _jwtSettings = jwtSettings.Value;
        _logger = logger;

        // Validate settings
        ValidateSettings();

        // Pre-compute signing credentials and validation parameters
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        _signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        _validationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = _jwtSettings.Audience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            RequireExpirationTime = true,
            RequireSignedTokens = true
        };
    }

    /// <inheritdoc />
    public Task<string> GenerateTokenAsync(
        string userId,
        string username,
        IEnumerable<string>? roles = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(userId);
        ArgumentException.ThrowIfNullOrWhiteSpace(username);

        return Task.Run(() => GenerateToken(userId, username, roles), cancellationToken);
    }

    /// <inheritdoc />
    public Task<ClaimsPrincipal?> ValidateTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        return Task.Run(() => ValidateToken(token), cancellationToken);
    }

    /// <inheritdoc />
    public async Task<string?> GetUserIdFromTokenAsync(
        string token,
        CancellationToken cancellationToken = default)
    {
        var principal = await ValidateTokenAsync(token, cancellationToken);
        return principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    }

    private string GenerateToken(string userId, string username, IEnumerable<string>? roles)
    {
        try
        {
            var claims = BuildClaims(userId, username, roles);
            var expiration = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: expiration,
                signingCredentials: _signingCredentials
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            _logger.LogInformation(
                "Token gerado com sucesso para usuário {Username} (ID: {UserId}). Expira em: {Expiration}",
                username, userId, expiration);

            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro ao gerar token para usuário {Username}", username);
            throw new InvalidOperationException("Falha ao gerar token JWT", ex);
        }
    }

    private ClaimsPrincipal? ValidateToken(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            // Validate token format first
            if (!tokenHandler.CanReadToken(token))
            {
                _logger.LogWarning("Token inválido: formato não reconhecido");
                return null;
            }

            var principal = tokenHandler.ValidateToken(
                token,
                _validationParameters,
                out var validatedToken);

            // Additional security check: ensure algorithm is correct
            if (validatedToken is JwtSecurityToken jwtToken &&
                !jwtToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Token rejeitado: algoritmo de assinatura inválido");
                return null;
            }

            _logger.LogDebug("Token validado com sucesso");
            return principal;
        }
        catch (SecurityTokenExpiredException ex)
        {
            _logger.LogWarning(ex, "Token expirado");
            return null;
        }
        catch (SecurityTokenInvalidSignatureException ex)
        {
            _logger.LogWarning(ex, "Assinatura do token inválida");
            return null;
        }
        catch (SecurityTokenException ex)
        {
            _logger.LogWarning(ex, "Token inválido: {Message}", ex.Message);
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erro inesperado ao validar token");
            return null;
        }
    }

    private static List<Claim> BuildClaims(string userId, string username, IEnumerable<string>? roles)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, userId),
            new(ClaimTypes.Name, username),
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64)
        };

        if (roles is not null)
        {
            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        }

        return claims;
    }

    private void ValidateSettings()
    {
        if (string.IsNullOrWhiteSpace(_jwtSettings.SecretKey))
            throw new InvalidOperationException("JWT SecretKey não pode ser vazio");

        if (_jwtSettings.SecretKey.Length < 32)
            throw new InvalidOperationException("JWT SecretKey deve ter no mínimo 32 caracteres");

        if (string.IsNullOrWhiteSpace(_jwtSettings.Issuer))
            throw new InvalidOperationException("JWT Issuer não pode ser vazio");

        if (string.IsNullOrWhiteSpace(_jwtSettings.Audience))
            throw new InvalidOperationException("JWT Audience não pode ser vazio");

        if (_jwtSettings.ExpirationMinutes <= 0)
            throw new InvalidOperationException("JWT ExpirationMinutes deve ser maior que zero");
    }
}