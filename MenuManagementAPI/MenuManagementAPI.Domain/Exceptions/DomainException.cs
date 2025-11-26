namespace MenuManagementAPI.Domain.Exceptions;

/// <summary>
/// Exceção base para erros de domínio
/// </summary>
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

/// <summary>
/// Exceção para entidade não encontrada
/// </summary>
public class NotFoundException : DomainException
{
    public NotFoundException(string entityName, object key) 
        : base($"{entityName} com ID '{key}' não foi encontrado.")
    {
    }
}

/// <summary>
/// Exceção para validação de negócio
/// </summary>
public class BusinessValidationException : DomainException
{
    public BusinessValidationException(string message) : base(message)
    {
    }
}
