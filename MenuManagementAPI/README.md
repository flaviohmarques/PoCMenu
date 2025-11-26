# Menu Management API - .NET Core 10

## 📋 Visão Geral

API RESTful desenvolvida em **.NET Core 10** utilizando **Clean Architecture**, **SQL Server**, **JWT**, **FluentValidation** e **Swagger/OpenAPI** para gerenciamento de menus.

Esta API faz parte da POC de migração do sistema legado em ASP Clássico para uma arquitetura moderna.

## 🏗️ Arquitetura

O projeto segue os princípios da **Clean Architecture**, dividido em 4 camadas:

```
MenuManagementAPI/
├── MenuManagementAPI.Domain/          # Camada de Domínio
│   ├── Entities/                      # Entidades de negócio
│   ├── Interfaces/                    # Contratos de repositórios
│   └── Exceptions/                    # Exceções customizadas
│
├── MenuManagementAPI.Application/     # Camada de Aplicação
│   ├── DTOs/                          # Data Transfer Objects
│   ├── Services/                      # Serviços de aplicação
│   ├── Validators/                    # Validadores FluentValidation
│   └── Common/                        # Padrões de resposta
│
├── MenuManagementAPI.Infrastructure/  # Camada de Infraestrutura
│   ├── Data/                          # DbContext e Configurações
│   ├── Repositories/                  # Implementação de repositórios
│   └── Services/                      # Serviços de infraestrutura (JWT)
│
└── MenuManagementAPI.Presentation/    # Camada de Apresentação
    ├── Controllers/                   # Controllers da API
    ├── Middleware/                    # Middlewares customizados
    └── Program.cs                     # Configuração da aplicação
```

### Princípios Aplicados

- **Separation of Concerns**: Cada camada tem responsabilidades bem definidas
- **Dependency Inversion**: Dependências apontam para abstrações
- **Single Responsibility**: Cada classe tem uma única responsabilidade
- **Interface Segregation**: Interfaces específicas para cada necessidade

## 🚀 Tecnologias

| Tecnologia | Versão | Uso |
|-----------|--------|-----|
| .NET Core | 10.0 | Framework principal |
| Entity Framework Core | 10.0 | ORM para acesso a dados |
| SQL Server | 2019+ | Banco de dados |
| FluentValidation | 12.1 | Validação de entrada |
| JWT | 8.14 | Autenticação |
| Swashbuckle | 10.0 | Documentação Swagger |

## 📦 Instalação

### Pré-requisitos

- .NET SDK 10.0 ou superior
- SQL Server 2019 ou superior
- Visual Studio 2022 ou VS Code

### Passos

1. **Clone o repositório**
```bash
git clone <repositório>
cd MenuManagementAPI
```

2. **Restaurar pacotes**
```bash
dotnet restore
```

3. **Configurar Connection String**

Edite o arquivo `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=SEU_SERVIDOR;Database=MenuManagementDB;User Id=SEU_USUARIO;Password=SUA_SENHA;TrustServerCertificate=True"
  }
}
```

4. **Criar banco de dados**
```bash
dotnet ef migrations add InitialCreate --project MenuManagementAPI.Infrastructure --startup-project MenuManagementAPI.Presentation
dotnet ef database update --project MenuManagementAPI.Infrastructure --startup-project MenuManagementAPI.Presentation
```

5. **Executar a aplicação**
```bash
cd MenuManagementAPI.Presentation
dotnet run
```

A API estará disponível em: `https://localhost:5001` ou `http://localhost:5000`

## 📚 Documentação da API

### Swagger UI

Acesse `https://localhost:5001` para visualizar a documentação interativa da API.

### Autenticação

A API utiliza **JWT (JSON Web Token)** para autenticação.

#### Obter Token

```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "admin",
  "password": "admin123"
}
```

**Resposta:**
```json
{
  "success": true,
  "message": "Login realizado com sucesso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "username": "admin",
    "expiresIn": 3600
  },
  "timestamp": "2025-11-17T10:00:00Z"
}
```

#### Usar Token

Adicione o token no header de todas as requisições protegidas:

```http
Authorization: Bearer {seu_token_aqui}
```

### Endpoints

#### Menus

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| GET | `/api/menus` | Lista todos os menus | Sim |
| GET | `/api/menus/search?nome={nome}` | Busca menus por nome | Sim |
| GET | `/api/menus/{id}` | Obtém menu por ID | Sim |
| POST | `/api/menus` | Cria novo menu | Sim |
| PUT | `/api/menus/{id}` | Atualiza menu | Sim |
| DELETE | `/api/menus/{id}` | Deleta menu | Sim |

#### Autenticação

| Método | Endpoint | Descrição | Auth |
|--------|----------|-----------|------|
| POST | `/api/auth/login` | Realiza login | Não |
| POST | `/api/auth/validate` | Valida token | Não |

## 📝 Exemplos de Uso

### Criar Menu

```http
POST /api/menus
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "Dashboard",
  "ordem": 1,
  "icone": "fa-home",
  "descricao": "Página inicial do sistema",
  "status": "Ativo"
}
```

**Resposta:**
```json
{
  "success": true,
  "message": "Menu criado com sucesso",
  "data": {
    "id": 1,
    "nome": "Dashboard",
    "ordem": 1,
    "icone": "fa-home",
    "descricao": "Página inicial do sistema",
    "status": "Ativo",
    "criadoEm": "2025-11-17T10:00:00Z",
    "atualizadoEm": "2025-11-17T10:00:00Z"
  },
  "timestamp": "2025-11-17T10:00:00Z"
}
```

### Buscar Menus

```http
GET /api/menus/search?nome=Dashboard
Authorization: Bearer {token}
```

**Resposta:**
```json
{
  "success": true,
  "message": "Busca realizada com sucesso",
  "data": [
    {
      "id": 1,
      "nome": "Dashboard",
      "ordem": 1,
      "icone": "fa-home",
      "descricao": "Página inicial do sistema",
      "status": "Ativo",
      "criadoEm": "2025-11-17T10:00:00Z",
      "atualizadoEm": "2025-11-17T10:00:00Z"
    }
  ],
  "timestamp": "2025-11-17T10:00:00Z"
}
```

### Atualizar Menu

```http
PUT /api/menus/1
Authorization: Bearer {token}
Content-Type: application/json

{
  "nome": "Dashboard Principal",
  "ordem": 1,
  "icone": "fa-home",
  "descricao": "Página inicial do sistema",
  "status": "Ativo"
}
```

### Deletar Menu

```http
DELETE /api/menus/1
Authorization: Bearer {token}
```

## 🔒 Segurança

### JWT

- **Algoritmo**: HS256
- **Expiração**: 60 minutos
- **Secret Key**: Configurável via `appsettings.json`

### Validação

- **FluentValidation**: Validação de entrada em todas as operações
- **Middleware Global**: Tratamento centralizado de exceções
- **CORS**: Configurado para permitir origens específicas

### Boas Práticas

- Senhas nunca são retornadas nas respostas
- Tokens expiram automaticamente
- Validação em múltiplas camadas (DTO, Domain, Database)
- Logging de erros para auditoria

## 🧪 Testes

### Executar Testes

```bash
dotnet test
```

### Cobertura de Testes

- Testes unitários para serviços
- Testes de integração para repositórios
- Testes de API para controllers

## 📊 Padrão de Resposta

Todas as respostas seguem o padrão `ApiResponse<T>`:

```json
{
  "success": true|false,
  "message": "Mensagem descritiva",
  "data": { ... },
  "errors": { ... },
  "timestamp": "2025-11-17T10:00:00Z"
}
```

### Sucesso

```json
{
  "success": true,
  "message": "Operação realizada com sucesso",
  "data": { ... },
  "timestamp": "2025-11-17T10:00:00Z"
}
```

### Erro de Validação

```json
{
  "success": false,
  "message": "Erro de validação",
  "errors": {
    "nome": ["O nome do menu é obrigatório"],
    "ordem": ["A ordem deve ser maior que zero"]
  },
  "timestamp": "2025-11-17T10:00:00Z"
}
```

### Erro de Negócio

```json
{
  "success": false,
  "message": "Já existe um menu com o nome 'Dashboard'",
  "timestamp": "2025-11-17T10:00:00Z"
}
```

## 🔧 Configuração

### appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MenuManagementDB;..."
  },
  "JwtSettings": {
    "SecretKey": "sua-chave-secreta-aqui",
    "Issuer": "MenuManagementAPI",
    "Audience": "MenuManagementClient",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Information"
    }
  }
}
```

## 🚀 Deploy

### Publicar para Produção

```bash
dotnet publish -c Release -o ./publish
```

### Docker

```dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY ./publish .
ENTRYPOINT ["dotnet", "MenuManagementAPI.Presentation.dll"]
```

## 📈 Performance

- **Entity Framework Core**: Queries otimizadas com índices
- **Async/Await**: Operações assíncronas em toda a aplicação
- **Caching**: Implementável via IMemoryCache ou Redis
- **Connection Pooling**: Gerenciamento eficiente de conexões

## 🐛 Troubleshooting

### Erro de Conexão com SQL Server

```bash
# Verificar se o SQL Server está rodando
sqlcmd -S localhost -U sa -P SuaSenha

# Testar connection string
dotnet ef database update --verbose
```

### Erro de Autenticação

- Verificar se o token não expirou
- Confirmar que o header `Authorization` está correto
- Validar a Secret Key no `appsettings.json`

- Documentação: `https://localhost:5001`


