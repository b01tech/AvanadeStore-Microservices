# AvanadeStore - Microservices Architecture

## 📋 Visão Geral

O **AvanadeStore** é uma aplicação de e-commerce desenvolvida utilizando arquitetura de microserviços com .NET 9. O projeto implementa as melhores práticas de desenvolvimento, incluindo Clean Architecture, Domain-Driven Design (DDD) e comunicação assíncrona entre serviços.

## 🏗️ Arquitetura

O sistema é composto por 4 microserviços principais:

### 🔐 Auth Service (Autenticação)
- **Responsabilidade**: Gerenciamento de usuários, autenticação e autorização
- **Funcionalidades**:
  - Cadastro e autenticação de clientes e funcionários
  - Gerenciamento de perfis de usuário
  - Operações CRUD completas (incluindo soft delete)
  - Validação de CPF e email únicos
- **Porta**: `5140`

### 📦 Inventory Service (Estoque)
- **Responsabilidade**: Gerenciamento de produtos e controle de estoque
- **Funcionalidades**:
  - Cadastro e gerenciamento de produtos
  - Controle de quantidades em estoque
  - Categorização de produtos
  - Consumo de mensagens de pedidos finalizados
- **Porta**: `5141`

### 💰 Sales Service (Vendas)
- **Responsabilidade**: Processamento de pedidos e vendas
- **Funcionalidades**:
  - Criação e gerenciamento de pedidos
  - Processamento de vendas com diferentes status
  - Histórico de transações
  - Publicação de eventos via RabbitMQ
- **Porta**: `5142`

### 🌐 Gateway Service (API Gateway)
- **Responsabilidade**: Ponto de entrada único para todos os serviços
- **Funcionalidades**:
  - Roteamento de requisições com Ocelot
  - Balanceamento de carga
  - Autenticação centralizada JWT
- **Porta**: `5000`

## 🛠️ Tecnologias Utilizadas

- **.NET 9** - Framework principal
- **Entity Framework Core** - ORM para acesso a dados
- **SQL Server** - Banco de dados relacional
- **RabbitMQ** - Message broker para comunicação assíncrona
- **Docker** - Containerização
- **JWT** - Autenticação e autorização
- **Scalar** - Documentação interativa da API
- **Tratamento Global de Exceções** - Middleware customizado para captura e tratamento de erros

## 📁 Estrutura do Projeto

```
AvanadeStore-Microservices/
├── AvanadeStore.Auth/           # Serviço de Autenticação
│   └── src/
│       ├── Auth.API/            # Camada de apresentação
│       ├── Auth.Application/    # Casos de uso e DTOs
│       ├── Auth.Domain/         # Entidades e regras de negócio
│       ├── Auth.Exception/      # Tratamento de exceções
│       └── Auth.Infra/          # Infraestrutura e dados
├── AvanadeStore.Inventory/      # Serviço de Estoque
│   └── src/
│       ├── Inventory.API/
│       ├── Inventory.Application/
│       ├── Inventory.Domain/
│       ├── Inventory.Exception/
│       └── Inventory.Infra/
├── AvanadeStore.Sales/          # Serviço de Vendas
│   └── src/
│       ├── Sales.API/
│       ├── Sales.Application/
│       ├── Sales.Domain/
│       ├── Sales.Exception/
│       └── Sales.Infra/
├── AvanadeStore.Gateway/        # API Gateway
│   ├── Diagrams/               # Diagramas da arquitetura
│   ├── docker-compose.yaml    # Orquestração dos containers
│   └── src/
│       └── Gateway.API/
└── LICENSE.md                  # Licença MIT
```

## 🚀 Como Executar

### Pré-requisitos

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)
- [SQL Server](https://www.microsoft.com/sql-server) (ou usar via Docker)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

### 1. Clone o Repositório

```bash
git clone <repository-url>
cd AvanadeStore-Microservices
```

### 2. Configuração com Docker (Recomendado)

#### Iniciar Infraestrutura (SQL Server + RabbitMQ)

```bash
cd AvanadeStore.Gateway
docker-compose up -d
```

Isso iniciará:
- **SQL Server** na porta `1433`
  - Usuário: `sa`
  - Senha: `AvanadeStore@2024`
- **RabbitMQ** nas portas `5672` (AMQP) e `15672` (Management UI)
  - Usuário: `admin`
  - Senha: `admin123`

#### Executar os Microserviços

```bash
# Auth Service (porta 5140)
cd AvanadeStore.Auth/src/Auth.API
dotnet run

# Inventory Service (porta 5141)
cd AvanadeStore.Inventory/src/Inventory.API
dotnet run

# Sales Service (porta 5142)
cd AvanadeStore.Sales/src/Sales.API
dotnet run

# Gateway Service (porta 5000)
cd AvanadeStore.Gateway/src/Gateway.API
dotnet run
```

### 3. Configuração Manual (Desenvolvimento)

#### Configurar Banco de Dados

1. Instale o SQL Server localmente
2. Atualize as connection strings nos arquivos `appsettings.json` de cada API
3. Execute as migrations:

```bash
# Para cada serviço
cd AvanadeStore.Auth/src/Auth.Infra
dotnet ef database update

cd AvanadeStore.Inventory/src/Inventory.Infra
dotnet ef database update

cd AvanadeStore.Sales/src/Sales.Infra
dotnet ef database update
```

## 📚 Documentação da API

Cada microserviço possui documentação interativa usando **Scalar**:

- **Auth Service**: `http://localhost:5140/scalar`
- **Inventory Service**: `http://localhost:5141/scalar`
- **Sales Service**: `http://localhost:5142/scalar`
- **Gateway Service**: `http://localhost:5000/scalar`

## 🔗 Endpoints Principais

### 🔐 Auth Service (`/auth`)

#### Clientes
- `POST /client` - Cadastro de cliente
- `PUT /client/{id}` - Atualização de cliente
- `DELETE /client/{id}` - Soft delete de cliente

#### Funcionários
- `POST /employee` - Cadastro de funcionário (Roles: Employee=1, Manager=2)
- `PUT /employee/{id}` - Atualização de funcionário
- `DELETE /employee/{id}` - Soft delete de funcionário (apenas Manager)

#### Login
- `POST /login/cpf` - Login com CPF
- `POST /login/email` - Login com email

### 📦 Inventory Service (`/inventory`)

#### Produtos
- `GET /product/list/{page}` - Lista produtos paginados 🔑
- `GET /product/{id}` - Busca produto por ID 🔑
- `POST /product` - Cria novo produto 🔑 (Role: Manager)

### 💰 Sales Service (`/sales`)

#### Pedidos
- `GET /order/list/{page}` - Lista todos os pedidos 🔑 (Roles: Employee, Manager)
- `GET /order/{id}` - Busca pedido por ID 🔑 (Roles: Employee, Manager)
- `GET /order/status/{status}/{page}` - Filtra pedidos por status 🔑 (Roles: Employee, Manager)
- `GET /order/my/{page}` - Pedidos do cliente logado 🔑 (Role: Client)
- `POST /order` - Cria novo pedido 🔑 (Role: Client)
- `PUT /order/{id}/confirm-separation` - Confirma separação 🔑 (Role: Employee)
- `PUT /order/{id}/cancel` - Cancela pedido 🔑 (Role: Client)
- `PUT /order/{id}/finish` - Finaliza pedido 🔑 (Role: Employee)

**Legenda**: 🔑 = Requer autenticação

## 🔧 Configuração

### Variáveis de Ambiente

Cada serviço pode ser configurado através de variáveis de ambiente ou arquivos `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost,1433;Database=AvanadeStore_Auth;User Id=sa;Password=AvanadeStore@2024;TrustServerCertificate=true;"
  },
  "RabbitMQ": {
    "HostName": "localhost",
    "Port": 5672,
    "UserName": "admin",
    "Password": "admin123"
  },
  "JWT": {
    "SecretKey": "your-secret-key-here",
    "Issuer": "AvanadeStore",
    "Audience": "AvanadeStore-Users"
  }
}
```

## 📊 Monitoramento

### RabbitMQ Management

Acesse o painel de gerenciamento do RabbitMQ em: `http://localhost:15672`

### Logs

Os logs são configurados usando o sistema de logging nativo do .NET e podem ser visualizados no console ou configurados para outros provedores.

## 🧪 Testes

```bash
# Executar todos os testes
dotnet test

# Executar testes de um projeto específico
cd AvanadeStore.Auth/src/Auth.Tests
dotnet test
```

## 📈 Diagramas

Os diagramas da arquitetura estão disponíveis na pasta `AvanadeStore.Gateway/Diagrams/`:

- **System Context**: Visão geral do sistema
- **Containers**: Arquitetura de containers
- **Fluxo de Pedido**: Fluxo de processamento de pedidos

## 🤝 Contribuição

1. Faça um fork do projeto
2. Crie uma branch para sua feature (`git checkout -b feature/AmazingFeature`)
3. Commit suas mudanças (`git commit -m 'Add some AmazingFeature'`)
4. Push para a branch (`git push origin feature/AmazingFeature`)
5. Abra um Pull Request

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE.md](LICENSE.md) para detalhes.

## 👥 Equipe

Desenvolvido pela equipe Avanade como projeto de demonstração de arquitetura de microserviços.

## 📞 Suporte

Para suporte e dúvidas, abra uma issue no repositório do projeto.