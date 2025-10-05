# Desafio Técnico - Microserviços

## 📌 Descrição do Desafio

Desenvolver uma aplicação com **arquitetura de microserviços** para gerenciamento de estoque de produtos e vendas em uma plataforma de e-commerce.  

O sistema será composto por **dois microserviços**:  

- **Gestão de Estoque**  
- **Gestão de Vendas**  

A comunicação será feita via **API Gateway** e **RabbitMQ**.

![Diagrama Projeto](./Docs/arquitetura.webp)

---

## 🏗️ Arquitetura Proposta

### **Microserviço 1 - Gestão de Estoque**

Responsável por:

- Cadastrar produtos.
- Controlar o estoque.
- Fornecer informações sobre quantidade disponível.

### **Microserviço 2 - Gestão de Vendas**

Responsável por:

- Gerenciar pedidos.
- Interagir com o serviço de estoque para verificar disponibilidade de produtos.
- Atualizar o estoque quando uma venda for confirmada.

### **API Gateway**

- Atua como ponto de entrada único.
- Realiza o roteamento das requisições para os microserviços corretos.

### **RabbitMQ**

- Comunicação assíncrona entre os microserviços.  
- Ex.: notificação de venda → atualização do estoque.

### **Autenticação (JWT)**

- Apenas usuários autenticados podem interagir com o sistema.  
- Protege os endpoints de estoque e vendas.

---

## ⚙️ Funcionalidades Requeridas

### **Microserviço 1 (Gestão de Estoque)**

- **Cadastro de Produtos**: nome, descrição, preço, quantidade.  
- **Consulta de Produtos**: catálogo e disponibilidade em estoque.  
- **Atualização de Estoque**: integração com vendas para reduzir a quantidade.

### **Microserviço 2 (Gestão de Vendas)**

- **Criação de Pedidos**: validação do estoque antes da compra.  
- **Consulta de Pedidos**: status dos pedidos realizados.  
- **Notificação de Venda**: envia evento ao estoque após confirmar pedido.

### **Comum aos dois**

- Autenticação via **JWT**.  
- Acesso centralizado via **API Gateway**.

---

## 💼 Contexto do Negócio

Simula um sistema de **e-commerce** para empresas que precisam:  

- Gerenciar estoque de produtos.  
- Realizar vendas de forma eficiente.  

A solução deve ser:  

- **Escalável**,  
- **Robusta**,  
- Com separação clara de responsabilidades.  

---

## 🛠️ Requisitos Técnicos

- **Tecnologia:** .NET Core (C#).  
- **Banco de Dados:** Relacional (SQL Server ou outro) com **Entity Framework**.  
- **Comunicação:** RabbitMQ (assíncrona).  
- **Autenticação:** JWT.  
- **API Gateway:** roteamento de requisições.  
- **Boas práticas:** RESTful API, tratamento de exceções e validações.

---

## ✅ Critérios de Aceitação

- Cadastro de produtos no microserviço de estoque.  
- Criação de pedidos com validação de estoque.  
- Comunicação entre microserviços via **RabbitMQ**.  
- Uso de **API Gateway** como ponto central de acesso.  
- Segurança via **JWT**.  
- Código bem estruturado e com boas práticas de POO.

---

## ⭐ Extras (Diferenciais)

- **Testes Unitários**: principais funcionalidades (cadastro de produtos, criação de pedidos).  
- **Monitoramento e Logs**: rastrear falhas e transações.  
- **Escalabilidade**: fácil adição de novos microserviços (ex.: pagamento, envio).  
