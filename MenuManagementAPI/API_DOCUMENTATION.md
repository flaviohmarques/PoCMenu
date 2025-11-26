# Menu Management API - Documentação Completa

## 📋 Índice

1. [Visão Geral](#visão-geral)
2. [Arquitetura](#arquitetura)
3. [Autenticação](#autenticação)
4. [Endpoints](#endpoints)
5. [Modelos de Dados](#modelos-de-dados)
6. [Códigos de Status](#códigos-de-status)
7. [Exemplos de Uso](#exemplos-de-uso)
8. [Tratamento de Erros](#tratamento-de-erros)

## Visão Geral

A **Menu Management API** é uma API RESTful desenvolvida em .NET Core 10 que permite gerenciar menus de um sistema. A API segue os princípios da Clean Architecture e implementa autenticação JWT, validação com FluentValidation e documentação via Swagger.

**Base URL:** `https://localhost:5001/api`

**Versão:** 1.0

**Formato de Resposta:** JSON

## Arquitetura

A API segue a **Clean Architecture** com 4 camadas bem definidas:

### Camada de Domínio (Domain)

Contém as entidades de negócio, interfaces de repositórios e exceções customizadas. Esta camada não possui dependências externas.

**Componentes:**
- `Menu`: Entidade principal representando um menu
- `IMenuRepository`: Interface para operações de dados
- `DomainException`, `NotFoundException`, `BusinessValidationException`: Exceções customizadas

### Camada de Aplicação (Application)

Contém a lógica de negócio, DTOs, validadores e serviços de aplicação.

**Componentes:**
- `MenuDto`, `CreateMenuDto`, `UpdateMenuDto`: Data Transfer Objects
- `IMenuService`, `MenuService`: Serviços de aplicação
- `CreateMenuValidator`, `UpdateMenuValidator`: Validadores FluentValidation
- `ApiResponse<T>`: Padrão de resposta customizado

### Camada de Infraestrutura (Infrastructure)

Implementa as interfaces definidas na camada de domínio e fornece acesso a recursos externos.

**Componentes:**
- `ApplicationDbContext`: Contexto do Entity Framework Core
- `MenuRepository`: Implementação do repositório
- `JwtService`: Serviço de geração e validação de tokens JWT

### Camada de Apresentação (Presentation)

Expõe a API via controllers HTTP e configura middlewares.

**Componentes:**
- `MenusController`: Endpoints para operações de menu
- `AuthController`: Endpoints para autenticação
- `GlobalExceptionMiddleware`: Tratamento global de exceções

## Autenticação

A API utiliza **JWT (JSON Web Token)** para autenticação. Todos os endpoints de menu requerem autenticação.

### Obter Token

**Endpoint:** `POST /api/auth/login`

**Request:**
```json
{
  "username": "admin",
  "password": "admin123"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Login realizado com sucesso",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6IjEiLCJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1lIjoiYWRtaW4iLCJqdGkiOiI4YzM0ZjY3OC0xMjM0LTQ1NjctODkwMS0xMjM0NTY3ODkwMTIiLCJleHAiOjE3MDA0ODQ4MDAsImlzcyI6Ik1lbnVNYW5hZ2VtZW50QVBJIiwiYXVkIjoiTWVudU1hbmFnZW1lbnRDbGllbnQifQ.abc123def456ghi789jkl012mno345pqr678stu901vwx234yz",
    "username": "admin",
    "expiresIn": 3600
  },
  "timestamp": "2025-11-17T22:30:00Z"
}
```

**Response (401 Unauthorized):**
```json
{
  "success": false,
  "message": "Usuário ou senha inválidos",
  "timestamp": "2025-11-17T22:30:00Z"
}
```

### Usar Token

Adicione o token no header `Authorization` de todas as requisições protegidas:

```
Authorization: Bearer {seu_token_aqui}
```

**Exemplo:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

### Validar Token

**Endpoint:** `POST /api/auth/validate`

**Request:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Token válido",
  "data": true,
  "timestamp": "2025-11-17T22:30:00Z"
}
```

## Endpoints

### Menus

#### Listar Todos os Menus

**Endpoint:** `GET /api/menus`

**Autenticação:** Requerida

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Menus obtidos com sucesso",
  "data": [
    {
      "id": 1,
      "nome": "Dashboard",
      "ordem": 1,
      "icone": "fa-home",
      "descricao": "Página inicial do sistema",
      "status": "Ativo",
      "criadoEm": "2025-11-17T22:00:00Z",
      "atualizadoEm": "2025-11-17T22:00:00Z"
    },
    {
      "id": 2,
      "nome": "Usuários",
      "ordem": 2,
      "icone": "fa-users",
      "descricao": "Gerenciamento de usuários",
      "status": "Ativo",
      "criadoEm": "2025-11-17T22:00:00Z",
      "atualizadoEm": "2025-11-17T22:00:00Z"
    }
  ],
  "timestamp": "2025-11-17T22:30:00Z"
}
```

#### Buscar Menus por Nome

**Endpoint:** `GET /api/menus/search?nome={nome}`

**Autenticação:** Requerida

**Parâmetros:**
- `nome` (query, opcional): Nome do menu para busca (busca parcial)

**Exemplos:**
- `/api/menus/search` - Retorna todos os menus
- `/api/menus/search?nome=Dashboard` - Busca menus que contenham "Dashboard"
- `/api/menus/search?nome=User` - Busca menus que contenham "User"

**Response (200 OK):**
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
      "criadoEm": "2025-11-17T22:00:00Z",
      "atualizadoEm": "2025-11-17T22:00:00Z"
    }
  ],
  "timestamp": "2025-11-17T22:30:00Z"
}
```

#### Obter Menu por ID

**Endpoint:** `GET /api/menus/{id}`

**Autenticação:** Requerida

**Parâmetros:**
- `id` (path, obrigatório): ID do menu

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Menu obtido com sucesso",
  "data": {
    "id": 1,
    "nome": "Dashboard",
    "ordem": 1,
    "icone": "fa-home",
    "descricao": "Página inicial do sistema",
    "status": "Ativo",
    "criadoEm": "2025-11-17T22:00:00Z",
    "atualizadoEm": "2025-11-17T22:00:00Z"
  },
  "timestamp": "2025-11-17T22:30:00Z"
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Menu com ID '999' não foi encontrado.",
  "timestamp": "2025-11-17T22:30:00Z"
}
```

#### Criar Novo Menu

**Endpoint:** `POST /api/menus`

**Autenticação:** Requerida

**Request Body:**
```json
{
  "nome": "Dashboard",
  "ordem": 1,
  "icone": "fa-home",
  "descricao": "Página inicial do sistema",
  "status": "Ativo"
}
```

**Campos:**
- `nome` (string, obrigatório): Nome do menu (máx. 255 caracteres)
- `ordem` (integer, obrigatório): Ordem de exibição (> 0)
- `icone` (string, obrigatório): Classe do ícone Font Awesome (máx. 255 caracteres)
- `descricao` (string, opcional): Descrição do menu (máx. 1000 caracteres)
- `status` (string, obrigatório): "Ativo" ou "Inativo"

**Response (201 Created):**
```json
{
  "success": true,
  "message": "Menu criado com sucesso",
  "data": {
    "id": 11,
    "nome": "Dashboard",
    "ordem": 1,
    "icone": "fa-home",
    "descricao": "Página inicial do sistema",
    "status": "Ativo",
    "criadoEm": "2025-11-17T22:30:00Z",
    "atualizadoEm": "2025-11-17T22:30:00Z"
  },
  "timestamp": "2025-11-17T22:30:00Z"
}
```

**Response (400 Bad Request - Validação):**
```json
{
  "success": false,
  "message": "Erro de validação",
  "errors": {
    "nome": ["O nome do menu é obrigatório"],
    "ordem": ["A ordem deve ser maior que zero"]
  },
  "timestamp": "2025-11-17T22:30:00Z"
}
```

**Response (400 Bad Request - Duplicado):**
```json
{
  "success": false,
  "message": "Já existe um menu com o nome 'Dashboard'",
  "timestamp": "2025-11-17T22:30:00Z"
}
```

#### Atualizar Menu

**Endpoint:** `PUT /api/menus/{id}`

**Autenticação:** Requerida

**Parâmetros:**
- `id` (path, obrigatório): ID do menu a ser atualizado

**Request Body:**
```json
{
  "nome": "Dashboard Principal",
  "ordem": 1,
  "icone": "fa-home",
  "descricao": "Página inicial do sistema atualizada",
  "status": "Ativo"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Menu atualizado com sucesso",
  "data": {
    "id": 1,
    "nome": "Dashboard Principal",
    "ordem": 1,
    "icone": "fa-home",
    "descricao": "Página inicial do sistema atualizada",
    "status": "Ativo",
    "criadoEm": "2025-11-17T22:00:00Z",
    "atualizadoEm": "2025-11-17T22:35:00Z"
  },
  "timestamp": "2025-11-17T22:35:00Z"
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Menu com ID '999' não foi encontrado.",
  "timestamp": "2025-11-17T22:35:00Z"
}
```

#### Deletar Menu

**Endpoint:** `DELETE /api/menus/{id}`

**Autenticação:** Requerida

**Parâmetros:**
- `id` (path, obrigatório): ID do menu a ser deletado

**Response (200 OK):**
```json
{
  "success": true,
  "message": "Menu deletado com sucesso",
  "data": true,
  "timestamp": "2025-11-17T22:40:00Z"
}
```

**Response (404 Not Found):**
```json
{
  "success": false,
  "message": "Menu com ID '999' não foi encontrado.",
  "timestamp": "2025-11-17T22:40:00Z"
}
```

## Modelos de Dados

### Menu

| Campo | Tipo | Obrigatório | Descrição |
|-------|------|-------------|-----------|
| id | integer | Sim (auto) | Identificador único |
| nome | string | Sim | Nome do menu (máx. 255 caracteres) |
| ordem | integer | Sim | Ordem de exibição (> 0) |
| icone | string | Sim | Classe do ícone Font Awesome (máx. 255 caracteres) |
| descricao | string | Não | Descrição do menu (máx. 1000 caracteres) |
| status | string | Sim | "Ativo" ou "Inativo" |
| criadoEm | datetime | Sim (auto) | Data/hora de criação (UTC) |
| atualizadoEm | datetime | Sim (auto) | Data/hora de atualização (UTC) |

### ApiResponse<T>

| Campo | Tipo | Descrição |
|-------|------|-----------|
| success | boolean | Indica se a operação foi bem-sucedida |
| message | string | Mensagem descritiva da resposta |
| data | T | Dados da resposta (tipo genérico) |
| errors | object | Erros de validação (opcional) |
| timestamp | datetime | Timestamp da resposta (UTC) |

## Códigos de Status

| Código | Descrição | Quando Ocorre |
|--------|-----------|---------------|
| 200 OK | Sucesso | Operação realizada com sucesso |
| 201 Created | Criado | Recurso criado com sucesso |
| 400 Bad Request | Requisição Inválida | Erro de validação ou regra de negócio |
| 401 Unauthorized | Não Autorizado | Token ausente, inválido ou expirado |
| 404 Not Found | Não Encontrado | Recurso não existe |
| 500 Internal Server Error | Erro Interno | Erro não tratado no servidor |

## Exemplos de Uso

### Exemplo 1: Fluxo Completo de Autenticação e Criação de Menu

```bash
# 1. Fazer login e obter token
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"admin","password":"admin123"}'

# Resposta: { "success": true, "data": { "token": "eyJ..." } }

# 2. Criar novo menu usando o token
curl -X POST https://localhost:5001/api/menus \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJ..." \
  -d '{
    "nome": "Novo Menu",
    "ordem": 11,
    "icone": "fa-star",
    "descricao": "Menu de teste",
    "status": "Ativo"
  }'
```

### Exemplo 2: Buscar e Atualizar Menu

```bash
# 1. Buscar menu por nome
curl -X GET "https://localhost:5001/api/menus/search?nome=Dashboard" \
  -H "Authorization: Bearer eyJ..."

# 2. Atualizar menu encontrado
curl -X PUT https://localhost:5001/api/menus/1 \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer eyJ..." \
  -d '{
    "nome": "Dashboard Atualizado",
    "ordem": 1,
    "icone": "fa-home",
    "descricao": "Descrição atualizada",
    "status": "Ativo"
  }'
```

### Exemplo 3: Listar Todos e Deletar

```bash
# 1. Listar todos os menus
curl -X GET https://localhost:5001/api/menus \
  -H "Authorization: Bearer eyJ..."

# 2. Deletar menu específico
curl -X DELETE https://localhost:5001/api/menus/10 \
  -H "Authorization: Bearer eyJ..."
```

## Tratamento de Erros

A API implementa um middleware global de tratamento de exceções que padroniza todas as respostas de erro.

### Tipos de Erro

#### 1. Erro de Validação (400)

Ocorre quando os dados enviados não passam nas validações do FluentValidation.

```json
{
  "success": false,
  "message": "Erro de validação",
  "errors": {
    "nome": ["O nome do menu é obrigatório"],
    "ordem": ["A ordem deve ser maior que zero"],
    "icone": ["O ícone do menu é obrigatório"]
  },
  "timestamp": "2025-11-17T22:30:00Z"
}
```

#### 2. Erro de Negócio (400)

Ocorre quando uma regra de negócio é violada.

```json
{
  "success": false,
  "message": "Já existe um menu com o nome 'Dashboard'",
  "timestamp": "2025-11-17T22:30:00Z"
}
```

#### 3. Recurso Não Encontrado (404)

Ocorre quando o recurso solicitado não existe.

```json
{
  "success": false,
  "message": "Menu com ID '999' não foi encontrado.",
  "timestamp": "2025-11-17T22:30:00Z"
}
```

#### 4. Não Autorizado (401)

Ocorre quando o token JWT está ausente, inválido ou expirado.

```json
{
  "success": false,
  "message": "Usuário ou senha inválidos",
  "timestamp": "2025-11-17T22:30:00Z"
}
```

#### 5. Erro Interno do Servidor (500)

Ocorre quando há um erro não tratado no servidor.

```json
{
  "success": false,
  "message": "Erro interno do servidor",
  "timestamp": "2025-11-17T22:30:00Z"
}
```

### Boas Práticas para Tratamento de Erros

1. **Sempre verifique o campo `success`** antes de processar `data`
2. **Exiba mensagens de erro amigáveis** ao usuário baseadas em `message`
3. **Para erros de validação**, itere sobre `errors` e exiba cada campo com seus erros
4. **Implemente retry logic** para erros 500 (com backoff exponencial)
5. **Renove o token JWT** quando receber erro 401
6. **Logue todos os erros** para auditoria e debugging

---

**Última Atualização:** 17 de Novembro de 2025

**Versão da API:** 1.0

**Contato:** contato@menumanagement.com
