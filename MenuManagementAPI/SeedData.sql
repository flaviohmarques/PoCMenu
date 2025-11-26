-- Script para popular o banco de dados com dados de exemplo
-- Menu Management API - POC

USE MenuManagementDB;
GO

-- Limpar dados existentes (opcional)
-- DELETE FROM Menus;
-- GO

-- Inserir menus de exemplo
INSERT INTO Menus (Nome, Ordem, Icone, Descricao, Status, CriadoEm, AtualizadoEm)
VALUES
    ('Dashboard', 1, 'fa-home', 'Página inicial do sistema', 1, GETUTCDATE(), GETUTCDATE()),
    ('Usuários', 2, 'fa-users', 'Gerenciamento de usuários', 1, GETUTCDATE(), GETUTCDATE()),
    ('Relatórios', 3, 'fa-bar-chart', 'Relatórios e análises', 1, GETUTCDATE(), GETUTCDATE()),
    ('Configurações', 4, 'fa-cog', 'Configurações do sistema', 1, GETUTCDATE(), GETUTCDATE()),
    ('Produtos', 5, 'fa-shopping-cart', 'Catálogo de produtos', 1, GETUTCDATE(), GETUTCDATE()),
    ('Financeiro', 6, 'fa-dollar', 'Gestão financeira', 1, GETUTCDATE(), GETUTCDATE()),
    ('Estoque', 7, 'fa-database', 'Controle de estoque', 1, GETUTCDATE(), GETUTCDATE()),
    ('Vendas', 8, 'fa-shopping-bag', 'Gestão de vendas', 1, GETUTCDATE(), GETUTCDATE()),
    ('Clientes', 9, 'fa-address-book', 'Cadastro de clientes', 1, GETUTCDATE(), GETUTCDATE()),
    ('Ajuda', 10, 'fa-question-circle', 'Central de ajuda', 2, GETUTCDATE(), GETUTCDATE());
GO

-- Verificar dados inseridos
SELECT * FROM Menus ORDER BY Ordem;
GO

PRINT 'Dados de exemplo inseridos com sucesso!';
GO
