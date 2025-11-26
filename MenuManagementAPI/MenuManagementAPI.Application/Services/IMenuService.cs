using MenuManagementAPI.Application.DTOs;

namespace MenuManagementAPI.Application.Services;

/// <summary>
/// Interface para serviço de Menu
/// </summary>
public interface IMenuService
{
    Task<IEnumerable<MenuDto>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<MenuDto>> SearchByNameAsync(string nome, CancellationToken cancellationToken = default);
    Task<MenuDto> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<MenuDto> CreateAsync(CreateMenuDto createMenuDto, CancellationToken cancellationToken = default);
    Task<MenuDto> UpdateAsync(int id, UpdateMenuDto updateMenuDto, CancellationToken cancellationToken = default);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default);
}
