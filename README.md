# WhatsApp Business DDD

![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=.net)
![C#](https://img.shields.io/badge/C%23-11-239120?logo=c-sharp)
![License](https://img.shields.io/badge/license-MIT-blue)

Sistema completo de integração com WhatsApp Business API seguindo Domain-Driven Design (DDD) para atendimento automatizado via IA e atendimento humano.

## 📋 Índice

- [Sobre o Projeto](#sobre-o-projeto)
- [Funcionalidades](#funcionalidades)
- [Arquitetura](#arquitetura)
- [Tecnologias](#tecnologias)
- [Pré-requisitos](#pré-requisitos)
- [Instalação](#instalação)
- [Configuração](#configuração)
- [Uso](#uso)
- [Estrutura do Projeto](#estrutura-do-projeto)
- [Testes](#testes)
- [Desenvolvimento](#desenvolvimento)
- [Contribuindo](#contribuindo)

## 🎯 Sobre o Projeto

Este projeto implementa uma aplicação completa para integração com WhatsApp Business API, oferecendo:

- 📱 **Recepção e resposta de mensagens** do WhatsApp
- 🔐 **Autenticação de usuários** via nome e telefone
- 🤖 **Atendimento automatizado** via IA (servidor MCP)
- 👨‍💼 **Sistema de atendimento humano** para operadores
- 💾 **Persistência completa** de conversas e mensagens
- 🔒 **Gerenciamento seguro** de configurações

## ✨ Funcionalidades

### Integração WhatsApp
- Recebimento de mensagens via webhook
- Envio de mensagens (texto, mídia, templates)
- Validação de assinatura de webhook
- Suporte a diferentes tipos de mensagem

### Autenticação de Usuários
- Validação de número de telefone
- Registro automático de novos usuários
- Verificação de autenticação

### IA e Atendimento Automatizado
- Integração com servidor MCP (Model Context Protocol)
- Respostas automáticas baseadas em IA
- Contexto de conversa mantido
- Transição suave para atendimento humano

### Atendimento Humano
- Dashboard real-time via SignalR
- Distribuição automática de conversas
- Visualização de histórico completo
- Múltiplos operadores simultâneos

### Segurança
- Criptografia de dados sensíveis (tokens, chaves API)
- Validação de webhooks (HMAC-SHA256)
- Configurações protegidas
- Suporte a HTTPS

## 🏗️ Arquitetura

O projeto segue os princípios de **Domain-Driven Design (DDD)** e **Clean Architecture**, dividido em 4 camadas:

```
┌─────────────────────────────────────┐
│         WebAPI Layer                │  ◄── Controllers, SignalR Hubs, Webhooks
├─────────────────────────────────────┤
│      Application Layer              │  ◄── Use Cases (CQRS), DTOs, Validators
├─────────────────────────────────────┤
│      Infrastructure Layer           │  ◄── EF Core, Repositories, External APIs
├─────────────────────────────────────┤
│         Domain Layer                │  ◄── Entities, Value Objects, Business Rules
└─────────────────────────────────────┘
```

### Domain Layer
- **Entities**: Conversation, Message, User, Operator, WhatsAppConfiguration
- **Value Objects**: PhoneNumber, MessageContent, ConversationStatus
- **Domain Services**: AuthenticationService, ConversationFlowService
- **Domain Events**: MessageReceivedEvent, UserAuthenticatedEvent, etc.

### Application Layer
- **CQRS com MediatR**: Separação de comandos e consultas
- **DTOs**: Objetos de transferência de dados
- **Validators**: FluentValidation para validação de entrada
- **AutoMapper**: Mapeamento entidade-DTO

### Infrastructure Layer
- **EF Core**: Acesso a dados com SQL Server
- **Repositories**: Implementações concretas
- **WhatsApp Client**: Cliente HTTP para WhatsApp Business API
- **MCP Client**: Cliente para servidor de IA
- **Encryption**: Serviços de criptografia

### WebAPI Layer
- **Controllers REST**: Endpoints para gerenciamento
- **SignalR Hub**: Comunicação real-time
- **Webhook**: Recepção de mensagens do WhatsApp
- **Middleware**: Tratamento de erros e logging

## 🛠️ Tecnologias

### Backend
- **.NET 9.0** - Framework principal
- **C# 11** - Linguagem de programação
- **ASP.NET Core** - Web API
- **Entity Framework Core 9** - ORM
- **SQL Server** - Banco de dados

### Bibliotecas Principais
- **MediatR** - CQRS pattern
- **FluentValidation** - Validação de entrada
- **AutoMapper** - Mapeamento de objetos
- **Serilog** - Logging estruturado
- **SignalR** - Comunicação real-time
- **Polly** - Resiliência HTTP

### Testes
- **xUnit** - Framework de testes
- **FluentAssertions** - Assertions fluentes
- **Moq** - Mocking
- **EF Core InMemory** - Banco em memória para testes

## 📋 Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server 2019+](https://www.microsoft.com/sql-server) ou [SQL Server Express](https://www.microsoft.com/sql-server/sql-server-downloads)
- [WhatsApp Business API](https://developers.facebook.com/docs/whatsapp/cloud-api) - Conta configurada
- Servidor MCP (opcional, para IA)

## 🚀 Instalação

### 1. Clone o repositório

```bash
git clone https://github.com/AdilsonMachado/whatsappbusinessddd.git
cd whatsappbusinessddd
```

### 2. Restaure as dependências

```bash
dotnet restore
```

### 3. Configure o banco de dados

Edite a connection string em `src/WhatsAppBusiness.WebAPI/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=WhatsAppBusinessDB;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 4. Execute as migrations

```bash
cd src/WhatsAppBusiness.WebAPI
dotnet ef database update
```

### 5. Build o projeto

```bash
dotnet build
```

## ⚙️ Configuração

### WhatsApp Business API

1. Acesse o [Meta for Developers](https://developers.facebook.com/)
2. Crie um app e configure WhatsApp Business API
3. Obtenha:
   - **Phone Number ID**
   - **Access Token**
   - **Webhook Verify Token** (você cria este)
   - **Business Account ID**

### Configuração do Aplicativo

Edite `src/WhatsAppBusiness.WebAPI/appsettings.json`:

```json
{
  "WhatsApp": {
    "PhoneNumberId": "SEU_PHONE_NUMBER_ID",
    "AccessToken": "SEU_ACCESS_TOKEN",
    "WebhookVerifyToken": "SEU_VERIFY_TOKEN",
    "BusinessAccountId": "SEU_BUSINESS_ACCOUNT_ID"
  },
  "MCP": {
    "ServerUrl": "http://localhost:3000",
    "ApiKey": "sua-chave-api-mcp"
  }
}
```

> ⚠️ **Importante**: Em produção, use variáveis de ambiente ou Azure Key Vault para armazenar credenciais.

### Configurar Webhook no Meta

1. No painel do Meta for Developers, vá em WhatsApp > Configuration
2. Configure o webhook:
   - **Callback URL**: `https://seu-dominio.com/api/webhook/whatsapp`
   - **Verify Token**: O mesmo definido em `WebhookVerifyToken`
3. Subscribe aos eventos: `messages`

## 🎮 Uso

### Iniciar a aplicação

```bash
cd src/WhatsAppBusiness.WebAPI
dotnet run
```

A aplicação estará disponível em:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- SignalR Hub: `https://localhost:5001/hubs/conversation`

### API Endpoints

#### Conversas
```
GET    /api/conversations              # Listar conversas ativas
GET    /api/conversations/{id}         # Obter conversa específica
POST   /api/conversations/{id}/assign  # Atribuir a operador
GET    /api/conversations/operator/{operatorId}  # Conversas do operador
```

#### Mensagens
```
GET    /api/messages/{conversationId}  # Mensagens da conversa
POST   /api/messages                   # Enviar mensagem
```

#### Operadores
```
GET    /api/operators                  # Listar operadores
GET    /api/operators/online           # Operadores online
POST   /api/operators/{id}/status      # Atualizar status
```

#### Webhook
```
GET    /api/webhook/whatsapp           # Verificação do webhook
POST   /api/webhook/whatsapp           # Receber mensagens
```

### SignalR Hub

Conecte-se ao hub para receber atualizações em tempo real:

```javascript
const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://localhost:5001/hubs/conversation")
    .build();

connection.on("NewMessage", (conversationId, message) => {
    console.log("Nova mensagem:", message);
});

await connection.start();
await connection.invoke("JoinConversation", conversationId);
```

### Health Check

Verifique a saúde da aplicação:

```
GET /health
```

## 📁 Estrutura do Projeto

```
whatsappbusinessddd/
├── src/
│   ├── WhatsAppBusiness.Domain/           # Camada de Domínio
│   │   ├── Entities/                      # Entidades de domínio
│   │   ├── ValueObjects/                  # Objetos de valor
│   │   ├── Services/                      # Serviços de domínio
│   │   ├── Events/                        # Eventos de domínio
│   │   └── Interfaces/                    # Interfaces de repositórios
│   │
│   ├── WhatsAppBusiness.Application/      # Camada de Aplicação
│   │   ├── DTOs/                          # Data Transfer Objects
│   │   ├── UseCases/                      # Casos de uso (CQRS)
│   │   │   ├── Commands/                  # Comandos
│   │   │   └── Queries/                   # Consultas
│   │   ├── Validators/                    # Validadores FluentValidation
│   │   ├── Mappings/                      # Perfis AutoMapper
│   │   └── Interfaces/                    # Interfaces de serviços
│   │
│   ├── WhatsAppBusiness.Infrastructure/   # Camada de Infraestrutura
│   │   ├── Data/                          # EF Core
│   │   │   ├── Configurations/            # Configurações de entidades
│   │   │   ├── Repositories/              # Implementações de repositórios
│   │   │   └── Migrations/                # Migrações
│   │   ├── WhatsApp/                      # Cliente WhatsApp Business API
│   │   ├── MCP/                           # Cliente MCP (IA)
│   │   ├── Configuration/                 # Gerenciamento de configuração
│   │   └── Security/                      # Serviços de segurança
│   │
│   └── WhatsAppBusiness.WebAPI/           # Camada de Apresentação
│       ├── Controllers/                   # Controllers REST
│       ├── Hubs/                          # SignalR Hubs
│       ├── Webhooks/                      # Webhooks
│       ├── Middleware/                    # Middleware personalizado
│       └── Extensions/                    # Extensões DI
│
└── tests/
    ├── WhatsAppBusiness.Domain.Tests/
    ├── WhatsAppBusiness.Application.Tests/
    └── WhatsAppBusiness.Infrastructure.Tests/
```

## 🧪 Testes

### Executar todos os testes

```bash
dotnet test
```

### Executar testes com cobertura

```bash
dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=opencover
```

## 🔧 Desenvolvimento

### Criar uma nova migration

```bash
cd src/WhatsAppBusiness.WebAPI
dotnet ef migrations add NomeDaMigration --project ../WhatsAppBusiness.Infrastructure
```

### Reverter uma migration

```bash
dotnet ef database update PreviousMigrationName --project ../WhatsAppBusiness.Infrastructure
```

## 🤝 Contribuindo

Contribuições são bem-vindas! Para contribuir:

1. Fork o projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

---

Feito com ❤️ usando .NET 9 e Domain-Driven Design
