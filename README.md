# CmCapitalDevSrProject

## 🧾 Visão Geral

Este projeto é a implementação do desafio técnico para desenvolvedor .NET Sênior, proposto pela CM Capital. Trata-se de uma API RESTful desenvolvida com ASP.NET Core que gerencia vendas de produtos, cadastro de clientes e produtos, controle de estoque, estornos, recomendações de produtos e geração de relatórios. A aplicação segue boas práticas de engenharia de software como separação de responsabilidades, injeção de dependência, autenticação via JWT, testes automatizados e documentação com Swagger.

> **Diferenciais implementados:**
> 
> ✅ Testes unitários com alta cobertura  
> ✅ Logging com Serilog + MongoDB
---

## 🔧 Tecnologias Utilizadas

- .NET 8
- Entity Framework Core
- SQL Server 2022
- MongoDB 6 (para logs com Serilog)
- JWT (autenticação)
- Swagger
- Serilog
- xUnit, Moq, FluentAssertions
- Docker + Docker Compose

---

## 🧱 Estrutura do Projeto

```bash
CmCapitalDevSrProject/
│
├── Controllers/         # Endpoints expostos da API
├── Models/              # Entidades do domínio
    ├── DTOs/            # Objetos de transferência
├── Repositories/        # Acesso a dados (implementações e interfaces)
│   ├── Data/            # Contexto do banco de dados (DbContext)
│   └── Interfaces/      # Contratos dos repositórios
├── Services/            # Lógica de negócio da aplicação
│   ├── Interfaces/      # Contratos dos serviços
│   └── Utils/           # Classes utilitárias
├── appsettings.json     # Configurações da aplicação
│
CmCapitalDevSrProject.Tests/
├── Fakes/           # Repositórios simulados
└── Services/        # Testes de serviços
```

---

## ⚙️ Como Executar Localmente (com Docker)

### 1. Pré-requisitos

- [Docker](https://www.docker.com/), [Docker Compose](https://docs.docker.com/compose/install/)

### 2. Subir o Ambiente Completo

Execute o seguinte comando no diretório raiz do projeto (onde está o `docker-compose.yml`):

```bash
docker-compose up --build
```

Esse comando irá:

- Subir um container com **MongoDB** (porta `27017`) para armazenar os logs do Serilog.
- Subir um container com **SQL Server 2022** (porta `1433`) com senha definida (`Str0ng@Senha123`).
- Construir e subir a aplicação **CmCapitalDevSrProject** na porta `8080`.

### 3. Aplicar Migrações

Ainda no terminal:

```bash
dotnet ef database update --project CmCapitalDevSrProject --startup-project CmCapitalDevSrProject --connection "Server=localhost,1433;Database=ClientesDb;User=sa;Password=Str0ng@Senha123;TrustServerCertificate=True;"
```

> **Obs:** Você também pode executar esse comando dentro do container da aplicação, se necessário.

### 5. Inserir Dados de Exemplo

Conecte-se no banco de dados com as credenciais da aplicação e Execute o script SQL"./TesteDevSrCmCapital/DbScripts/Inserts.txt"  com os dados iniciais.

---

## 🔎 Acessando a API

A API estará disponível em:

- Swagger: [http://localhost:8080/swagger](http://localhost:8080/swagger)

---

## 🧪 Executar Testes

No host (fora do container), ainda no diretório raiz do projeto (onde está o `docker-compose.yml`):

```bash
dotnet test
```

---
## Login com credenicais fictícias

Para efeito de exemplificação de fluxo de autenticação usando JWT, apenas um usuário de exemplo foi criado

```json
{
    "Usuario": "TestUser",
    "Senha": "123456"
}
```


---

## 📜 Regras de Negócio Implementadas

### Clientes
- Cadastro com saldo inicial ≥ 0

### Produtos
- Cadastro com preço ≥ 0, estoque ≥ 0, vencimento > hoje
- Filtros por preço, estoque e vencimento
- Auditoria de alterações

### Vendas
- Validação de saldo e estoque
- Estorno em até 7 dias
- Notificação de baixo estoque
- Sugestões de produtos (baseado em saldo, vencimento e categoria)

### Relatórios
- Agrupamento por produto e período

---

## 📋 Documentação da API

Disponível via Swagger:  
👉 [http://localhost:8080/swagger](http://localhost:8080/swagger)

---
## 📝 Observações

- Importante: Essa aplicação precisa de mais ajustes para rodar e ambiente produtivo. Tudo foi construído para testes locais
- Logs são enviados ao MongoDB via Serilog.
- Autenticação JWT protege os endpoints.
- O projeto segue GitFlow para versionamento.

---

## 📈 Melhorias Futuras

- Frontend em Angular (diferencial sugerido)
- Testes de integração com banco real
- Endpoint para monitoramento de logs
- Mais endpoints de consultas, produtos filtrados, cliente filtrados, vendas e mudanças nos produtos

---
