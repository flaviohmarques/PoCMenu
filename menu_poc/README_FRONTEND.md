# Menu Management System - Front-end React

## Visão Geral

Front-end em React integrado com API .NET Core 10 para gerenciamento de menus.

## Requisitos

- **Node.js**: 18.x ou superior
- **pnpm**: 8.x ou superior (gerenciador de pacotes)

## Instalação

### 1. Instalar pnpm (se não tiver)

**Windows:**
```powershell
npm install -g pnpm
```

**Linux/Mac:**
```bash
npm install -g pnpm
# ou
curl -fsSL https://get.pnpm.io/install.sh | sh -
```

### 2. Instalar dependências

```bash
cd menu_poc
pnpm install
```

**IMPORTANTE:** 
- Use `pnpm install` ao invés de `npm install`. O projeto foi configurado com pnpm e não funcionará corretamente com npm devido a incompatibilidades de versões.
- O projeto já inclui `cross-env` para compatibilidade com Windows, Linux e Mac.

### 3. Configurar variáveis de ambiente

O projeto já vem com um arquivo `.env.local` pré-configurado:

```env
VITE_API_URL=https://localhost:7241/api
```

Se sua API .NET Core estiver rodando em outra porta, edite o arquivo `.env.local` e altere a URL conforme necessário.

### 4. Executar o projeto

**Modo desenvolvimento:**
```bash
pnpm dev
```

O front-end estará disponível em: `http://localhost:3000`

**Build para produção:**
```bash
pnpm build
```

Os arquivos compilados estarão em `dist/`

## Credenciais de Teste

```
Usuário: admin
Senha: admin123
```

## Estrutura do Projeto

```
menu_poc/
├── client/                 # Código do front-end
│   ├── src/
│   │   ├── config/        # Configurações da API
│   │   ├── services/      # Serviços (auth, menu)
│   │   ├── pages/         # Páginas (Login, SearchMenus, CadastroMenu)
│   │   ├── components/    # Componentes reutilizáveis
│   │   └── App.tsx        # Rotas e proteção
│   └── index.html         # HTML principal com ícones customizados
├── .env.local            # Configuração da URL da API (local)
├── .env.production       # Configuração da URL da API (produção)
└── package.json
```

## Funcionalidades

### Autenticação
- Login com JWT
- Proteção de rotas
- Logout automático quando token expira

### Gerenciamento de Menus
- **Listar**: Visualizar todos os menus
- **Buscar**: Filtrar menus por nome
- **Criar**: Adicionar novo menu
- **Editar**: Atualizar menu existente
- **Deletar**: Remover menu

## Fluxo de Uso

1. Acesse `http://localhost:3000`
2. Será redirecionado para `/login`
3. Digite as credenciais: `admin` / `admin123`
4. Após login, acesse a tela de busca de menus
5. Use os botões para criar, editar ou deletar menus

## Tecnologias

- **React 19**: Framework front-end
- **TypeScript**: Tipagem estática
- **Vite**: Build tool
- **Axios**: Cliente HTTP
- **Wouter**: Roteamento
- **Tailwind CSS**: Estilização
- **shadcn/ui**: Componentes de UI
- **Sonner**: Notificações toast

## Troubleshooting

### Erro: "Cannot find module"

**Solução:**
```bash
rm -rf node_modules
pnpm install
```

### Erro: "Network Error" ao fazer requisições

**Causa:** API não está rodando ou URL incorreta

**Solução:**
1. Verifique se a API .NET Core está rodando
2. Verifique a URL no arquivo `.env.local`
3. Verifique se o CORS está configurado na API

### Erro: "401 Unauthorized"

**Causa:** Token inválido ou expirado

**Solução:**
1. Faça logout e login novamente
2. Verifique se a API está usando a mesma Secret Key

### Porta 3000 já está em uso

**Solução:**
```bash
# Linux/Mac
lsof -ti:3000 | xargs kill -9

# Windows
netstat -ano | findstr :3000
taskkill /PID <PID> /F
```

## Scripts Disponíveis

```bash
pnpm dev          # Inicia servidor de desenvolvimento
pnpm build        # Build para produção
pnpm preview      # Preview do build de produção
pnpm check        # Executa linter
```

## Integração com API .NET Core

O front-end se comunica com a API através dos seguintes endpoints:

### Autenticação
- `POST /api/auth/login` - Login
- `POST /api/auth/validate` - Validar token

### Menus
- `GET /api/menu` - Listar todos
- `GET /api/menu/search?nome={nome}` - Buscar por nome
- `GET /api/menu/{id}` - Obter por ID
- `POST /api/menu` - Criar
- `PUT /api/menu/{id}` - Atualizar
- `DELETE /api/menu/{id}` - Deletar

Todos os endpoints (exceto login) requerem autenticação JWT no header:
```
Authorization: Bearer <token>
```

## Customização

### Ícones e Título

O projeto já vem configurado com:
- **Título**: "Menus"
- **Ícone**: Ícone de menu customizado

Para alterar, edite o arquivo `client/index.html`:
```html
<link rel="icon" type="image/png" href="URL_DO_SEU_ICONE" />
<title>Seu Título</title>
```

### URL da API

Para alterar a URL da API, edite o arquivo `.env.local`:
```env
VITE_API_URL=https://sua-api.com/api
```

## Notas Importantes

1. **Use pnpm**: O projeto foi configurado com pnpm e não funcionará com npm
2. **Configure a API URL**: O arquivo `.env.local` já está configurado para `https://localhost:7241/api`
3. **CORS**: Certifique-se de que o CORS está configurado na API para aceitar requisições do front-end
4. **HTTPS**: Em produção, use HTTPS tanto no front-end quanto na API

## Licença

Este projeto é uma Prova de Conceito (POC) para migração de ASP Clássico para arquitetura moderna.

--
