namespace MenuManagementAPI.Application.DTOs;

/// <summary>
/// DTO para resposta de Menu
/// </summary>
public class MenuDto
{
    public int Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public string Icone { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CriadoEm { get; set; }
    public DateTime AtualizadoEm { get; set; }
}

/// <summary>
/// DTO para criação de Menu
/// </summary>
public class CreateMenuDto
{
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public string Icone { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Status { get; set; } = "Ativo";
}

/// <summary>
/// DTO para atualização de Menu
/// </summary>
public class UpdateMenuDto
{
    public string Nome { get; set; } = string.Empty;
    public int Ordem { get; set; }
    public string Icone { get; set; } = string.Empty;
    public string? Descricao { get; set; }
    public string Status { get; set; } = "Ativo";
}

/// <summary>
/// DTO para busca de Menu
/// </summary>
public class SearchMenuDto
{
    public string? Nome { get; set; }
}
