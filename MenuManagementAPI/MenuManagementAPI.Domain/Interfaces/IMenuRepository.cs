using MenuManagementAPI.Domain.Entities;

namespace MenuManagementAPI.Domain.Interfaces;

/// <summary>
/// Interface para operações de repositório de Menu
/// </summary>
public interface IMenuRepository
{
    /// <summary>
    /// Obtém todos os menus
    /// </summary>
    Task<IEnumerable<Menu>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Busca menus por nome
    /// </summary>
    Task<IEnumerable<Menu>> SearchByNameAsync(string nome, CancellationToken cancellationToken = default);

    /// <summary>
    /// Obtém um menu pelo ID
    /// </summary>
    Task<Menu?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Verifica se um menu com o nome já existe
    /// </summary>
    Task<bool> ExistsByNameAsync(string nome, int? excludeId = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cria um novo menu
    /// </summary>
    Task<Menu> CreateAsync(Menu menu, CancellationToken cancellationToken = default);

    /// <summary>
    /// Atualiza um menu existente
    /// </summary>
    Task<Menu> UpdateAsync(Menu menu, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deleta um menu
    /// </summary>
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
