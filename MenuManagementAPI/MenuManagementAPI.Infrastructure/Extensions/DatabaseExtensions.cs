using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MenuManagementAPI.Infrastructure.Data;
using MenuManagementAPI.Domain.Entities;

namespace MenuManagementAPI.Infrastructure.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    /// Initializes the database based on environment
    /// </summary>
    public static async Task InitializeDatabaseAsync(this IServiceProvider serviceProvider, bool isDevelopment)
    {
        if (isDevelopment)
        {
            await InitializeInMemoryDatabaseAsync(serviceProvider);
        }
        else
        {
            await ApplyMigrationsAsync(serviceProvider);
        }
    }

    /// <summary>
    /// Initialize and seed in-memory database
    /// </summary>
    private static async Task InitializeInMemoryDatabaseAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseInitialization");

            var created = await context.Database.EnsureCreatedAsync();

            if (created)
            {
                logger.LogInformation("In-Memory database criado com sucesso");
            }

            if (await context.Menus.AnyAsync())
            {
                logger.LogInformation("In-Memory database já contém dados ({Count} menus)",
                    await context.Menus.CountAsync());
                return;
            }

            await SeedDevelopmentDataAsync(context, logger);
        }
        catch (Exception ex)
        {
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseInitialization");
            logger.LogError(ex, "Erro ao inicializar In-Memory database");
            throw;
        }
    }

    /// <summary>
    /// Apply EF Core migrations for SQL Server
    /// </summary>
    private static async Task ApplyMigrationsAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var services = scope.ServiceProvider;

        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseMigration");

            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation("Aplicando {Count} migrações pendentes...", pendingMigrations.Count());

                foreach (var migration in pendingMigrations)
                {
                    logger.LogInformation("Migração: {Migration}", migration);
                }

                await context.Database.MigrateAsync();
                logger.LogInformation("Migrações aplicadas com sucesso");
            }
            else
            {
                logger.LogInformation("Nenhuma migração pendente");
            }

            var appliedMigrations = await context.Database.GetAppliedMigrationsAsync();
            logger.LogInformation("Total de migrações aplicadas: {Count}", appliedMigrations.Count());
        }
        catch (Exception ex)
        {
            var loggerFactory = services.GetRequiredService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("DatabaseMigration");
            logger.LogError(ex, "Erro ao aplicar migrações");
            throw;
        }
    }

    /// <summary>
    /// Seed sample data for development
    /// </summary>
    private static async Task SeedDevelopmentDataAsync(ApplicationDbContext context, ILogger logger)
    {
        var sampleMenus = new[]
        {
            new Menu
            {
                Nome = "Dashboard",
                Descricao = "Página principal do sistema",
                Icone = "dashboard",
                Ordem = 1,
                Status = MenuStatus.Ativo,
                CriadoEm = DateTime.UtcNow,
                AtualizadoEm = DateTime.UtcNow
            },
            new Menu
            {
                Nome = "Usuários",
                Descricao = "Gerenciamento de usuários",
                Icone = "users",
                Ordem = 2,
                Status = MenuStatus.Ativo,
                CriadoEm = DateTime.UtcNow,
                AtualizadoEm = DateTime.UtcNow
            },
            new Menu
            {
                Nome = "Relatórios",
                Descricao = "Visualização de relatórios",
                Icone = "chart",
                Ordem = 3,
                Status = MenuStatus.Ativo,
                CriadoEm = DateTime.UtcNow,
                AtualizadoEm = DateTime.UtcNow
            },
            new Menu
            {
                Nome = "Configurações",
                Descricao = "Configurações do sistema",
                Icone = "settings",
                Ordem = 4,
                Status = MenuStatus.Inativo,
                CriadoEm = DateTime.UtcNow,
                AtualizadoEm = DateTime.UtcNow
            },
            new Menu
            {
                Nome = "Produtos",
                Descricao = "Catálogo de produtos",
                Icone = "shopping-cart",
                Ordem = 5,
                Status = MenuStatus.Ativo,
                CriadoEm = DateTime.UtcNow,
                AtualizadoEm = DateTime.UtcNow
            }
        };

        await context.Menus.AddRangeAsync(sampleMenus);
        await context.SaveChangesAsync();

        logger.LogInformation("In-Memory database populado com {Count} menus de exemplo", sampleMenus.Length);
    }
}