namespace MenuManagementAPI.Domain.Entities;

/// <summary>
/// Representa um menu do sistema
/// </summary>
public class Menu
{
    /// <summary>
    /// Identificador único do menu
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Nome do menu
    /// </summary>
    public string Nome { get; set; } = string.Empty;

    /// <summary>
    /// Ordem de exibição do menu
    /// </summary>
    public int Ordem { get; set; }

    /// <summary>
    /// Classe do ícone (ex: fa-home, fa-users)
    /// </summary>
    public string Icone { get; set; } = string.Empty;

    /// <summary>
    /// Descrição do menu
    /// </summary>
    public string? Descricao { get; set; }

    /// <summary>
    /// Status do menu (ativo ou inativo)
    /// </summary>
    public MenuStatus Status { get; set; } = MenuStatus.Ativo;

    /// <summary>
    /// Data de criação
    /// </summary>
    public DateTime CriadoEm { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Data de atualização
    /// </summary>
    public DateTime AtualizadoEm { get; set; } = DateTime.UtcNow;
}

/// <summary>
/// Enum para status do menu
/// </summary>
public enum MenuStatus
{
    /// <summary>
    /// Menu ativo
    /// </summary>
    Ativo = 1,

    /// <summary>
    /// Menu inativo
    /// </summary>
    Inativo = 2
}
