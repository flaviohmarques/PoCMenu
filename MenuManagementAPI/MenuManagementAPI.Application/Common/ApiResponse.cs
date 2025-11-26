namespace MenuManagementAPI.Application.Common;

/// <summary>
/// Padrão de resposta da API
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Indica se a operação foi bem-sucedida
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Mensagem da resposta
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// Dados da resposta
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Erros de validação
    /// </summary>
    public Dictionary<string, string[]>? Errors { get; set; }

    /// <summary>
    /// Timestamp da resposta
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Cria uma resposta de sucesso
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data, string? message = null)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message ?? "Operação realizada com sucesso",
            Data = data
        };
    }

    /// <summary>
    /// Cria uma resposta de erro
    /// </summary>
    public static ApiResponse<T> ErrorResponse(string message, Dictionary<string, string[]>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
