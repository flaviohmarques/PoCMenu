# Menu Management API - Guia de Deployment

## 📋 Índice

1. [Pré-requisitos](#pré-requisitos)
2. [Configuração do Ambiente](#configuração-do-ambiente)
3. [Deployment Local](#deployment-local)
4. [Deployment em IIS](#deployment-em-iis)
5. [Deployment com Docker](#deployment-com-docker)
6. [Deployment em Azure](#deployment-em-azure)
7. [Configurações de Produção](#configurações-de-produção)
8. [Monitoramento e Logs](#monitoramento-e-logs)
9. [Troubleshooting](#troubleshooting)

## Pré-requisitos

### Servidor

- **Sistema Operacional**: Windows Server 2019+ ou Linux (Ubuntu 20.04+)
- **RAM**: Mínimo 2GB, recomendado 4GB+
- **Disco**: Mínimo 10GB de espaço livre
- **.NET Runtime**: .NET 10.0 ou superior
- **Banco de Dados**: SQL Server 2019+ ou Azure SQL Database

### Ferramentas de Desenvolvimento

- .NET SDK 10.0+
- SQL Server Management Studio (SSMS) ou Azure Data Studio
- Git
- Visual Studio 2022 ou VS Code (opcional)

## Configuração do Ambiente

### 1. Instalar .NET Runtime

#### Windows

```powershell
# Download e instale o .NET 10.0 Runtime
# https://dotnet.microsoft.com/download/dotnet/10.0

# Verificar instalação
dotnet --version
```

#### Linux (Ubuntu)

```bash
# Adicionar repositório Microsoft
wget https://packages.microsoft.com/config/ubuntu/22.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
sudo dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Instalar .NET Runtime
sudo apt-get update
sudo apt-get install -y aspnetcore-runtime-10.0

# Verificar instalação
dotnet --version
```

### 2. Configurar SQL Server

#### Criar Banco de Dados

```sql
-- Conecte-se ao SQL Server e execute:
CREATE DATABASE MenuManagementDB;
GO

-- Criar usuário para a aplicação (opcional)
CREATE LOGIN MenuAppUser WITH PASSWORD = 'SuaSenhaSegura123!';
GO

USE MenuManagementDB;
GO

CREATE USER MenuAppUser FOR LOGIN MenuAppUser;
GO

GRANT SELECT, INSERT, UPDATE, DELETE ON SCHEMA::dbo TO MenuAppUser;
GO
```

#### Connection String

```
Server=SEU_SERVIDOR;Database=MenuManagementDB;User Id=MenuAppUser;Password=SuaSenhaSegura123!;TrustServerCertificate=True;MultipleActiveResultSets=true
```

## Deployment Local

### 1. Clonar Repositório

```bash
git clone <repositório>
cd MenuManagementAPI
```

### 2. Configurar appsettings.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=MenuManagementDB;User Id=sa;Password=SuaSenha;TrustServerCertificate=True"
  },
  "JwtSettings": {
    "SecretKey": "SuaChaveSecretaAqui_MinimoDe32Caracteres!",
    "Issuer": "MenuManagementAPI",
    "Audience": "MenuManagementClient",
    "ExpirationMinutes": 60
  }
}
```

### 3. Aplicar Migrations

```bash
cd MenuManagementAPI.Presentation
dotnet ef database update --project ../MenuManagementAPI.Infrastructure
```

### 4. Popular Banco de Dados (Opcional)

```bash
# Execute o script SQL
sqlcmd -S localhost -U sa -P SuaSenha -d MenuManagementDB -i ../SeedData.sql
```

### 5. Executar Aplicação

```bash
dotnet run
```

Acesse: `https://localhost:5001`

## Deployment em IIS

### 1. Publicar Aplicação

```bash
cd MenuManagementAPI.Presentation
dotnet publish -c Release -o ./publish
```

### 2. Configurar IIS

#### Instalar Módulo ASP.NET Core

- Download: https://dotnet.microsoft.com/permalink/dotnetcore-current-windows-runtime-bundle-installer
- Execute o instalador

#### Criar Site no IIS

1. Abra o **IIS Manager**
2. Clique com botão direito em **Sites** → **Add Website**
3. Configure:
   - **Site name**: MenuManagementAPI
   - **Physical path**: Caminho para pasta `publish`
   - **Binding**: HTTP, porta 80 (ou HTTPS, porta 443)
4. Clique em **OK**

#### Configurar Application Pool

1. Vá em **Application Pools**
2. Selecione o pool da aplicação
3. Configure:
   - **.NET CLR Version**: No Managed Code
   - **Managed Pipeline Mode**: Integrated
4. Clique em **OK**

### 3. Configurar Permissões

```powershell
# Dar permissão de leitura para o usuário IIS
icacls "C:\caminho\para\publish" /grant "IIS_IUSRS:(OI)(CI)R" /T
```

### 4. Configurar web.config

O arquivo `web.config` é gerado automaticamente, mas você pode personalizá-lo:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <system.webServer>
    <handlers>
      <add name="aspNetCore" path="*" verb="*" modules="AspNetCoreModuleV2" resourceType="Unspecified" />
    </handlers>
    <aspNetCore processPath="dotnet" 
                arguments=".\MenuManagementAPI.Presentation.dll" 
                stdoutLogEnabled="true" 
                stdoutLogFile=".\logs\stdout" 
                hostingModel="inprocess" />
  </system.webServer>
</configuration>
```

### 5. Testar

Acesse: `http://seu-servidor/`

## Deployment com Docker

### 1. Criar Dockerfile

```dockerfile
# Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["MenuManagementAPI.Presentation/MenuManagementAPI.Presentation.csproj", "MenuManagementAPI.Presentation/"]
COPY ["MenuManagementAPI.Application/MenuManagementAPI.Application.csproj", "MenuManagementAPI.Application/"]
COPY ["MenuManagementAPI.Domain/MenuManagementAPI.Domain.csproj", "MenuManagementAPI.Domain/"]
COPY ["MenuManagementAPI.Infrastructure/MenuManagementAPI.Infrastructure.csproj", "MenuManagementAPI.Infrastructure/"]
RUN dotnet restore "MenuManagementAPI.Presentation/MenuManagementAPI.Presentation.csproj"
COPY . .
WORKDIR "/src/MenuManagementAPI.Presentation"
RUN dotnet build "MenuManagementAPI.Presentation.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "MenuManagementAPI.Presentation.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "MenuManagementAPI.Presentation.dll"]
```

### 2. Criar docker-compose.yml

```yaml
version: '3.8'

services:
  api:
    build: .
    ports:
      - "5000:80"
      - "5001:443"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ConnectionStrings__DefaultConnection=Server=sqlserver;Database=MenuManagementDB;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True
    depends_on:
      - sqlserver
    networks:
      - menu-network

  sqlserver:
    image: mcr.microsoft.com/mssql/server:2022-latest
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong@Passw0rd
    ports:
      - "1433:1433"
    volumes:
      - sqlserver-data:/var/opt/mssql
    networks:
      - menu-network

volumes:
  sqlserver-data:

networks:
  menu-network:
    driver: bridge
```

### 3. Build e Run

```bash
# Build imagem
docker-compose build

# Iniciar containers
docker-compose up -d

# Verificar logs
docker-compose logs -f api

# Aplicar migrations
docker-compose exec api dotnet ef database update --project ../MenuManagementAPI.Infrastructure
```

### 4. Testar

Acesse: `http://localhost:5000`

## Deployment em Azure

### 1. Criar Recursos no Azure

#### Via Azure Portal

1. **Criar Resource Group**
   - Nome: `rg-menu-management`
   - Região: `Brazil South`

2. **Criar Azure SQL Database**
   - Server name: `sql-menu-management`
   - Database name: `MenuManagementDB`
   - Pricing tier: Basic ou Standard

3. **Criar App Service**
   - Name: `app-menu-management`
   - Runtime stack: .NET 10
   - Region: `Brazil South`

#### Via Azure CLI

```bash
# Login
az login

# Criar Resource Group
az group create --name rg-menu-management --location brazilsouth

# Criar SQL Server
az sql server create \
  --name sql-menu-management \
  --resource-group rg-menu-management \
  --location brazilsouth \
  --admin-user sqladmin \
  --admin-password YourStrong@Passw0rd

# Criar SQL Database
az sql db create \
  --resource-group rg-menu-management \
  --server sql-menu-management \
  --name MenuManagementDB \
  --service-objective S0

# Criar App Service Plan
az appservice plan create \
  --name plan-menu-management \
  --resource-group rg-menu-management \
  --sku B1 \
  --is-linux

# Criar Web App
az webapp create \
  --resource-group rg-menu-management \
  --plan plan-menu-management \
  --name app-menu-management \
  --runtime "DOTNETCORE:10.0"
```

### 2. Configurar Connection String

```bash
az webapp config connection-string set \
  --resource-group rg-menu-management \
  --name app-menu-management \
  --settings DefaultConnection="Server=tcp:sql-menu-management.database.windows.net,1433;Database=MenuManagementDB;User ID=sqladmin;Password=YourStrong@Passw0rd;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;" \
  --connection-string-type SQLAzure
```

### 3. Deploy da Aplicação

```bash
# Publicar
dotnet publish -c Release -o ./publish

# Criar arquivo zip
cd publish
zip -r ../app.zip .
cd ..

# Deploy via Azure CLI
az webapp deployment source config-zip \
  --resource-group rg-menu-management \
  --name app-menu-management \
  --src app.zip
```

### 4. Aplicar Migrations

```bash
# Obter connection string
az sql db show-connection-string \
  --client ado.net \
  --server sql-menu-management \
  --name MenuManagementDB

# Aplicar migrations localmente apontando para Azure
dotnet ef database update --project MenuManagementAPI.Infrastructure --startup-project MenuManagementAPI.Presentation --connection "ConnectionString"
```

### 5. Testar

Acesse: `https://app-menu-management.azurewebsites.net`

## Configurações de Produção

### appsettings.Production.json

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Connection string de produção"
  },
  "JwtSettings": {
    "SecretKey": "ChaveSecretaDeProducaoMuitoSegura123456789!",
    "Issuer": "MenuManagementAPI",
    "Audience": "MenuManagementClient",
    "ExpirationMinutes": 60
  },
  "Logging": {
    "LogLevel": {
      "Default": "Warning",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Error"
    }
  },
  "AllowedHosts": "seudominio.com"
}
```

### Variáveis de Ambiente

```bash
# Windows
setx ASPNETCORE_ENVIRONMENT "Production"

# Linux
export ASPNETCORE_ENVIRONMENT=Production
```

### HTTPS e Certificados

#### Gerar Certificado Self-Signed (Desenvolvimento)

```bash
dotnet dev-certs https --trust
```

#### Usar Certificado em Produção

```json
{
  "Kestrel": {
    "Endpoints": {
      "Https": {
        "Url": "https://*:443",
        "Certificate": {
          "Path": "/path/to/certificate.pfx",
          "Password": "certificatePassword"
        }
      }
    }
  }
}
```

## Monitoramento e Logs

### Application Insights (Azure)

```bash
# Adicionar pacote
dotnet add package Microsoft.ApplicationInsights.AspNetCore

# Configurar no Program.cs
builder.Services.AddApplicationInsightsTelemetry();
```

### Serilog

```bash
# Adicionar pacotes
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File

# Configurar no Program.cs
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();
```

### Health Checks

```csharp
// Program.cs
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

app.MapHealthChecks("/health");
```

## Troubleshooting

### Erro: "Unable to connect to SQL Server"

**Solução:**
1. Verificar se o SQL Server está rodando
2. Testar connection string com `sqlcmd`
3. Verificar firewall do servidor
4. Confirmar credenciais

### Erro: "401 Unauthorized"

**Solução:**
1. Verificar se o token JWT está sendo enviado
2. Confirmar que a Secret Key é a mesma em todos os ambientes
3. Verificar se o token não expirou

### Erro: "500 Internal Server Error"

**Solução:**
1. Verificar logs da aplicação
2. Habilitar `DetailedErrors` em desenvolvimento
3. Verificar se todas as migrations foram aplicadas

### Performance Lenta

**Solução:**
1. Adicionar índices no banco de dados
2. Implementar caching (Redis)
3. Otimizar queries do Entity Framework
4. Escalar recursos do servidor

---

**Última Atualização:** 17 de Novembro de 2025
