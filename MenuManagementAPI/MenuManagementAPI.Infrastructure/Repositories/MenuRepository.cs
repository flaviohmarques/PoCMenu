using Microsoft.EntityFrameworkCore;
using MenuManagementAPI.Domain.Entities;
using MenuManagementAPI.Domain.Interfaces;
using MenuManagementAPI.Infrastructure.Data;

namespace MenuManagementAPI.Infrastructure.Repositories;

/// <summary>
/// Implementação do repositório de Menu
/// </summary>
public class MenuRepository(ApplicationDbContext context) : IMenuRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task<IEnumerable<Menu>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Menus
            .OrderBy(m => m.Ordem)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Menu>> SearchByNameAsync(string nome, CancellationToken cancellationToken = default)
    {
        var query = _context.Menus.AsQueryable();

        if (!string.IsNullOrWhiteSpace(nome))
        {
            query = query.Where(m => m.Nome.Contains(nome));
        }

        return await query
            .OrderBy(m => m.Ordem)
            .ToListAsync(cancellationToken);
    }

    public async Task<Menu?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _context.Menus
            .FirstOrDefaultAsync(m => m.Id == id, cancellationToken);
    }

    public async Task<bool> ExistsByNameAsync(string nome, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Menus.Where(m => m.Nome == nome);

        if (excludeId.HasValue)
        {
            query = query.Where(m => m.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<Menu> CreateAsync(Menu menu, CancellationToken cancellationToken = default)
    {
        _context.Menus.Add(menu);
        await _context.SaveChangesAsync(cancellationToken);
        return menu;
    }

    public async Task<Menu> UpdateAsync(Menu menu, CancellationToken cancellationToken = default)
    {
        _context.Menus.Update(menu);
        await _context.SaveChangesAsync(cancellationToken);
        return menu;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var menu = await GetByIdAsync(id, cancellationToken);
        if (menu == null)
        {
            return false;
        }

        _context.Menus.Remove(menu);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
