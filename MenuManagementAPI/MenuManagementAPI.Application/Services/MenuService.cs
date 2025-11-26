using MenuManagementAPI.Application.DTOs;
using MenuManagementAPI.Domain.Entities;
using MenuManagementAPI.Domain.Exceptions;
using MenuManagementAPI.Domain.Interfaces;

namespace MenuManagementAPI.Application.Services;

/// <summary>
/// Implementação do serviço de Menu
/// </summary>
public class MenuService : IMenuService
{
    private readonly IMenuRepository _menuRepository;

    public MenuService(IMenuRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<IEnumerable<MenuDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var menus = await _menuRepository.GetAllAsync(cancellationToken);
        return menus.Select(MapToDto);
    }

    public async Task<IEnumerable<MenuDto>> SearchByNameAsync(string nome, CancellationToken cancellationToken = default)
    {
        var menus = await _menuRepository.SearchByNameAsync(nome, cancellationToken);
        return menus.Select(MapToDto);
    }

    public async Task<MenuDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var menu = await _menuRepository.GetByIdAsync(id, cancellationToken);
        return menu == null ? throw new NotFoundException(nameof(Menu), id) : MapToDto(menu);
    }

    public async Task<MenuDto> CreateAsync(CreateMenuDto createMenuDto, CancellationToken cancellationToken = default)
    {
        // Verificar se já existe menu com o mesmo nome
        if (await _menuRepository.ExistsByNameAsync(createMenuDto.Nome, null, cancellationToken))
        {
            throw new BusinessValidationException($"Já existe um menu com o nome '{createMenuDto.Nome}'");
        }

        var menu = new Menu
        {
            Nome = createMenuDto.Nome,
            Ordem = createMenuDto.Ordem,
            Icone = createMenuDto.Icone,
            Descricao = createMenuDto.Descricao,
            Status = createMenuDto.Status == "Ativo" ? MenuStatus.Ativo : MenuStatus.Inativo,
            CriadoEm = DateTime.UtcNow,
            AtualizadoEm = DateTime.UtcNow
        };

        var createdMenu = await _menuRepository.CreateAsync(menu, cancellationToken);
        return MapToDto(createdMenu);
    }

    public async Task<MenuDto> UpdateAsync(int id, UpdateMenuDto updateMenuDto, CancellationToken cancellationToken = default)
    {
        var menu = await _menuRepository.GetByIdAsync(id, cancellationToken) ?? throw new NotFoundException(nameof(Menu), id);

        // Verificar se já existe outro menu com o mesmo nome
        if (await _menuRepository.ExistsByNameAsync(updateMenuDto.Nome, id, cancellationToken))
        {
            throw new BusinessValidationException($"Já existe outro menu com o nome '{updateMenuDto.Nome}'");
        }

        menu.Nome = updateMenuDto.Nome;
        menu.Ordem = updateMenuDto.Ordem;
        menu.Icone = updateMenuDto.Icone;
        menu.Descricao = updateMenuDto.Descricao;
        menu.Status = updateMenuDto.Status == "Ativo" ? MenuStatus.Ativo : MenuStatus.Inativo;
        menu.AtualizadoEm = DateTime.UtcNow;

        var updatedMenu = await _menuRepository.UpdateAsync(menu, cancellationToken);
        return MapToDto(updatedMenu);
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var menu = await _menuRepository.GetByIdAsync(id, cancellationToken);
        if (menu == null)
        {
            throw new NotFoundException(nameof(Menu), id);
        }

        return await _menuRepository.DeleteAsync(id, cancellationToken);
    }

    private static MenuDto MapToDto(Menu menu)
    {
        return new MenuDto
        {
            Id = menu.Id,
            Nome = menu.Nome,
            Ordem = menu.Ordem,
            Icone = menu.Icone,
            Descricao = menu.Descricao,
            Status = menu.Status == MenuStatus.Ativo ? "Ativo" : "Inativo",
            CriadoEm = menu.CriadoEm,
            AtualizadoEm = menu.AtualizadoEm
        };
    }
}
