# AvanadeStore - Microservices Architecture

## 📋 Visão Geral

O **AvanadeStore** é uma aplicação de e-commerce desenvolvida utilizando arquitetura de microserviços com .NET 9.
O projeto implementa as melhores práticas de desenvolvimento, incluindo Clean Architecture, Domain-Driven Design (DDD) e comunicação assíncrona entre serviços.

## 🏗️ Arquitetura

O sistema é composto por 4 microserviços principais:
![AvanadeStore-Microservices-Containers](C:\dev\AvanadeStore-Microservices\Diagrams\AvanadeStore-Microservices-Containers.drawio.png)

### 🔐 Auth Service (Autenticação)

-   **Responsabilidade**: Gerenciamento de usuários, autenticação e autorização
-   **Funcionalidades**:
    -   Cadastro e autenticação de clientes e funcionários
    -   Gerenciamento de perfis de usuário
    -   Operações CRUD completas (incluindo soft delete)
    -   Validação de CPF e email únicos
-   **Porta**: `5140`

### 📦 Inventory Service (Estoque)

-   **Responsabilidade**: Gerenciamento de produtos e controle de estoque
-   **Funcionalidades**:
    -   Cadastro e gerenciamento de produtos
    -   Controle de quantidades em estoque
    -   Categorização de produtos
    -   Consumo de mensagens de pedidos finalizados
-   **Porta**: `5141`

### 💰 Sales Service (Vendas)

-   **Responsabilidade**: Processamento de pedidos e vendas
-   **Funcionalidades**:
    -   Criação e gerenciamento de pedidos
    -   Processamento de vendas com diferentes status
    -   Histórico de transações
    -   Publicação de eventos via RabbitMQ
-   **Porta**: `5142`

### 🌐 Gateway Service (API Gateway)

-   **Responsabilidade**: Ponto de entrada único para todos os serviços
-   **Funcionalidades**:
    -   Roteamento de requisições com Ocelot
    -   Balanceamento de carga
    -   Autenticação centralizada JWT
-   **Porta**: `5000`

## 🛠️ Tecnologias Utilizadas

-   **.NET 9** - Framework principal
-   **Entity Framework Core** - ORM para acesso a dados
-   **SQL Server** - Banco de dados relacional
-   **RabbitMQ** - Message broker para comunicação assíncrona
-   **Docker** - Containerização
-   **JWT** - Autenticação e autorização
-   **Scalar** - Documentação interativa da API
-   **Tratamento Global de Exceções** - Middleware customizado para captura e tratamento de erros

## 📁 Estrutura do Projeto

```
AvanadeStore-Microservices/
├── AvanadeStore.Auth/           # Serviço de Autenticação
│   ├── src/
│   │   ├── Auth.API/            # Camada de apresentação
│   │   ├── Auth.Application/    # Casos de uso e DTOs
│   │   ├── Auth.Domain/         # Entidades e regras de negócio
│   │   ├── Auth.Exception/      # Tratamento de exceções
│   │   └── Auth.Infra/          # Infraestrutura e dados
│   └── test/
│       └── Auth.Tests/          # Testes (unitários/integração)
├── AvanadeStore.Inventory/      # Serviço de Estoque
│   ├── src/
│   │   ├── Inventory.API/
│   │   ├── Inventory.Application/
│   │   ├── Inventory.Domain/
│   │   ├── Inventory.Exception/
│   │   └── Inventory.Infra/
│   └── test/
│       └── Inventory.Tests/
├── AvanadeStore.Sales/          # Serviço de Vendas
│   ├── src/
│   │   ├── Sales.API/
│   │   ├── Sales.Application/
│   │   ├── Sales.Domain/
│   │   ├── Sales.Exception/
│   │   └── Sales.Infra/
│   └── test/
│       └── Sales.Tests/
├── AvanadeStore.Gateway/        # API Gateway
│   ├── src/
│   │   └── Gateway.API/
│   ├── Monitoring/              # Prometheus/Grafana configs
│   ├── Diagrams/                # Diagramas da arquitetura
│   └── docker-compose.yaml      # Orquestração de infraestrutura
├── LICENSE.md                   # Licença MIT
└── README.md
```

## 🚀 Como Executar

### Pré-requisitos

-   [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
-   [Docker Desktop](https://www.docker.com/products/docker-desktop)
-   [SQL Server](https://www.microsoft.com/sql-server) (ou usar via Docker)
-   [Visual Studio 2022](https://visualstudio.microsoft.com/) ou [VS Code](https://code.visualstudio.com/)

### Clone o Repositório

```bash
git clone <repository-url>
cd AvanadeStore-Microservices
```

### 🔧 Configuração

Atualize as connection strings nos arquivos `appsettings.json` de cada API e variáveis de ambiente

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

OBS: As migrations pendentes são executadas automaticamente ao iniciar os serviços

### Configuração com Docker (Recomendado)

#### Iniciar Infraestrutura (SQL Server + RabbitMQ)

```bash
cd AvanadeStore.Gateway
docker-compose up -d
```

Isso iniciará:

-   **SQL Server** na porta `1433`
    -   Usuário: `sa`
    -   Senha: `AvanadeStore@2024`
-   **RabbitMQ** nas portas `5672` (AMQP) e `15672` (Management UI)
    -   Usuário: `admin`
    -   Senha: `admin123`

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

## 📚 Documentação da API

Cada microserviço possui documentação interativa usando **Scalar**:

-   **Auth Service**: `http://localhost:5140/scalar`
-   **Inventory Service**: `http://localhost:5141/scalar`
-   **Sales Service**: `http://localhost:5142/scalar`

## 🔗 Endpoints Principais

### 🔐 Auth Service (`/auth`)

#### Clientes

-   `POST /client` - Cadastro de cliente
-   `PUT /client/{id}` - Atualização de cliente
-   `DELETE /client/{id}` - Soft delete de cliente

#### Funcionários

-   `POST /employee` - Cadastro de funcionário (Roles: Employee=1, Manager=2)
-   `PUT /employee/{id}` - Atualização de funcionário
-   `DELETE /employee/{id}` - Soft delete de funcionário (apenas Manager)

#### Login

-   `POST /login/cpf` - Login com CPF
-   `POST /login/email` - Login com email

### 📦 Inventory Service (`/inventory`)

#### Produtos

-   `GET /product/list/{page}` - Lista produtos paginados 🔑
-   `GET /product/{id}` - Busca produto por ID 🔑
-   `POST /product` - Cria novo produto 🔑 (Role: Manager)

### 💰 Sales Service (`/sales`)

#### Pedidos

-   `GET /order/list/{page}` - Lista todos os pedidos 🔑 (Roles: Employee, Manager)
-   `GET /order/{id}` - Busca pedido por ID 🔑 (Roles: Employee, Manager)
-   `GET /order/status/{status}/{page}` - Filtra pedidos por status 🔑 (Roles: Employee, Manager)
-   `GET /order/my/{page}` - Pedidos do cliente logado 🔑 (Role: Client)
-   `POST /order` - Cria novo pedido 🔑 (Role: Client)
-   `PUT /order/{id}/confirm-separation` - Confirma separação 🔑 (Role: Employee)
-   `PUT /order/{id}/cancel` - Cancela pedido 🔑 (Role: Client)
-   `PUT /order/{id}/finish` - Finaliza pedido 🔑 (Role: Employee)

**Legenda**: 🔑 = Requer autenticação

##

### Variáveis de Ambiente

Cada serviço pode ser configurado através de variáveis de ambiente ou arquivos `appsettings.json`:

## 📊 Monitoramento

### RabbitMQ Management

Acesse o painel de gerenciamento do RabbitMQ em: `http://localhost:15672`

### Prometheus

Acesse o painel de monitoramento do Prometheus em: `http://localhost:9090`

### Grafana

Acesse o painel de monitoramento do Grafana em: `http://localhost:3000`

![dashboard-grafana-avanadestore-example](C:\dev\AvanadeStore-Microservices\Diagrams\dashbord-grafana-avanadestore-example.jpg)

### Logs

Os logs são configurados usando o sistema de logging nativo do .NET e podem ser visualizados no console ou configurados para outros provedores.

## 🧪 Testes

Instruções detalhadas para executar e inspecionar os testes de cada serviço.

```bash
# Executar todos os testes da solução (saída minimal)
dotnet test AvanadeStore.sln -v minimal

# Executar testes por serviço
cd AvanadeStore.Auth/test/Auth.Tests
dotnet test -v minimal

cd ../../AvanadeStore.Inventory/test/Inventory.Tests
dotnet test -v minimal

cd ../../AvanadeStore.Sales/test/Sales.Tests
dotnet test -v minimal

# Filtrar testes (exemplos)
dotnet test --filter TestCategory=Unit
dotnet test --filter FullyQualifiedName~Namespace.DoSeuTeste

# Coletar cobertura (opcional)
dotnet test --collect "XPlat Code Coverage" --results-directory ./TestResults
```

Logs mais verbosos: use `-l "console;verbosity=detailed"` para investigar falhas.

## 📈 Diagramas

Os diagramas da arquitetura estão disponíveis na pasta `./Diagrams/`:

-   **System Context**: Visão geral do sistema
-   **Containers**: Arquitetura de containers
-   **Fluxo de Pedido**: Fluxo de processamento de pedidos
-   **Entidades**: Diagrama de entidades e relacionamentos do banco de dados

Links rápidos:

-   `./Diagrams/AvanadeStore-Microservices-System-Context.drawio.png`
-   `./Diagrams/AvanadeStore-Microservices-Containers.drawio.png`
-   `./Diagrams/AvanadeStore-Microservices-Fluxo-pedido.drawio.png`
-   `./Diagrams/AvanadeStore-Microservices-ERD.drawio.png`

## 🔌 Gateway – Rotas e Testes de Endpoints

-   Base URL: `http://localhost:5000`
-   Mapeamentos (Ocelot):
    -   `http://localhost:5000/auth/*` → `http://localhost:5140/*`
    -   `http://localhost:5000/inventory/*` → `http://localhost:5141/*`
    -   `http://localhost:5000/sales/*` → `http://localhost:5142/*`
-   Autenticação: enviar `Authorization: Bearer <token>` nas rotas protegidas.

Exemplos de testes (curl):

```bash
# Login por email via Gateway (obtém token JWT)
curl -X POST http://localhost:5000/auth/login/email \
  -H "Content-Type: application/json" \
  -d '{"email":"john.doe@avanade.com","password":"ChangeMe123!"}'

# Listar produtos (paginado) via Gateway
curl -X GET http://localhost:5000/inventory/product/list/1 \
  -H "Authorization: Bearer <token>"

# Criar pedido via Gateway
curl -X POST http://localhost:5000/sales/order \
  -H "Authorization: Bearer <token>" \
  -H "Content-Type: application/json" \
  -d '{"items":[{"productId":1,"quantity":2,"price":10.00}]}'

# Confirmar separação de pedido
curl -X PUT http://localhost:5000/sales/order/<orderId>/confirm-separation \
  -H "Authorization: Bearer <token>"
```

Observações:

-   Alguns endpoints requerem perfis específicos (ex.: Manager para criação de produto).
-   As rotas de documentação interativa (Scalar) estão disponíveis em cada serviço: `http://localhost:<porta>/scalar`.

## 📄 Licença

Este projeto está licenciado sob a Licença MIT - veja o arquivo [LICENSE.md](LICENSE.md) para detalhes.

## Observações

Desenvolvido como projeto pessoal de demonstração de arquitetura de microserviços.
