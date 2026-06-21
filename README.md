# FCG Catalog API

# **Descrição do projeto**

O FCG Catalog API é o microsserviço desenvolvido em .NET 8 responsável pelo gerenciamento do catálogo de jogos da plataforma FCG (FIAP Cloud Games). A aplicação segue os princípios de Clean Architecture, separação de responsabilidades e boas práticas de desenvolvimento backend.

Este serviço é responsável por:

* Cadastro de jogos
* Consulta de jogos
* Atualização de jogos
* Remoção lógica de jogos
* Gerenciamento da biblioteca de jogos dos usuários
* Inicialização do fluxo de compra de jogos
* Publicação de eventos relacionados ao processo de compra
* Persistência de dados
* Validação de regras de negócio

# Responsabilidades do Microsserviço

### Gerenciamento de Jogos

Permite realizar operações de cadastro, consulta, atualização e remoção lógica de jogos disponíveis na plataforma.

### Controle de Acesso

O gerenciamento do catálogo é restrito a usuários com perfil administrativo.

### Fluxo de Compra

Permite iniciar o processo de compra de um jogo.

Ao iniciar uma compra com sucesso, o serviço publicará o evento:

```text
OrderPlacedEvent
```

Esse evento será consumido pelo microsserviço de Pagamentos para processamento da compra.

### Biblioteca de Jogos

Após a confirmação do pagamento, o jogo será associado à biblioteca do usuário.

# **Regras de Negócio**

De acordo com os requisitos do desafio, o sistema implementa as seguintes regras:

### **Jogo**

* O jogo deve possuir Nome, Descrição e Preço.

### **Validação de Nome**

* O nome do jogo é obrigatório.
* O nome deve possuir no mínimo 8 caracteres.

### **Validação de Descrição**

* A descrição do jogo é obrigatória.
* A descrição deve possuir no mínimo 8 caracteres.

### **Cadastro de Jogos**

* Apenas usuários com perfil Admin podem cadastrar jogos.
* Não é permitido cadastrar jogos com dados inválidos.

### **Atualização de Jogos**

* Apenas usuários com perfil Admin podem atualizar jogos existentes.

### **Remoção de Jogos**

* A remoção é lógica através da propriedade:

```text
IsActive = false
```

### **Consulta de Jogos**

* Usuários autenticados podem consultar os jogos cadastrados.

### **Compra de Jogos**

* Usuários autenticados podem iniciar a compra de jogos.
* Cada requisição representa a compra de um único jogo.
* O processo de pagamento será realizado pelo microsserviço de Pagamentos.

---

## **Recursos do Projeto**

* **Serilog**: Para geração e gerenciamento de logs.
* **FluentValidation**: Para validação de dados e regras de negócios.
* **Entity Framework Core (ORM)**: Para mapeamento e interação com o banco de dados.
* **Unit of Work**: Padrão de design para gerenciamento de transações.
* **Migrations**: Gerenciamento de alterações no banco de dados.
* **Xunit**: Para criação de testes unitários.
* **FluentAssertions**: Melhor legibilidade nas validações.
* **JWT Bearer Authentication**: Validação de tokens gerados pelo UsersAPI.
* **Swagger/OpenAPI**: Documentação automatizada dos endpoints.
* **Docker**: Containerização da aplicação e banco de dados.

---

## **Variáveis de Ambiente**

A aplicação utiliza as seguintes variáveis de ambiente:

| Variável                          | Descrição                                                |
| --------------------------------- | -------------------------------------------------------- |
| ConnectionStrings__WebApiDatabase | String de conexão com o SQL Server                       |
| JwtSettings__SecretKey            | Chave utilizada para validação do token JWT              |
| JwtSettings__Issuer               | Emissor do token JWT                                     |
| JwtSettings__Audience             | Público do token JWT                                     |
| RunMigrations                     | Define se as migrations serão executadas automaticamente |

---

## **Como Executar o Projeto**

### **1. Configuração Inicial do Banco de Dados**

1. Faça o clone do projeto.
2. Configure a string de conexão do banco de dados.
3. Execute a aplicação.

As migrations serão aplicadas automaticamente durante a inicialização quando a configuração `RunMigrations` estiver habilitada.

---

### **2. Executando o Projeto**

1. Abra o projeto no Visual Studio 2022 ou em outro IDE de sua escolha.

2. Configure o projeto principal para execução:

   * Clique com o botão direito no projeto **FCG.CatalogAPI** e selecione `Set as Startup Project`.

3. Clique no botão **HTTPS** para iniciar a aplicação.

---

### **3. Execução com Docker**

#### **Pré-requisitos**

* Docker Desktop instalado.

#### **Executando a Aplicação**

Na raiz do projeto execute:

```bash
docker compose up --build
```

Esse comando irá:

* Construir a imagem da API.
* Criar o container da aplicação.
* Criar o container do SQL Server.
* Executar as migrations automaticamente (quando configurado).

#### **Acessando a API**

Após a inicialização dos containers, a documentação Swagger estará disponível em:

```text
http://localhost:8080/swagger
```

#### **Parando os Containers**

```bash
docker compose down
```

---

### **4. Banco de Dados**

* **Centralização de Exceções:**
  Implementada a classe `ExceptionMiddleware` para unificar o tratamento de erros.

* **Mensagens de Erro:**

Se o banco de dados estiver indisponível:

```text
The database is currently unavailable. Please try again later.
```

Para erros inesperados:

```text
An unexpected error occurred. Please contact support if the problem persists.
```

---

### **5. Configuração do Log**

* O sistema gera logs estruturados diariamente.

* O log será salvo no diretório configurado pela aplicação.

**Formato do arquivo de log criado:**

* Arquivo diário contendo informações estruturadas de execução.

---

### **6. Endpoints Disponíveis**

#### Jogos

```http
POST   /api/games
GET    /api/games/all
GET    /api/games/{id}
PUT    /api/games/{id}
DELETE /api/games/{id}
```

#### Compra de Jogos

```http
POST /api/games/{gameId}/purchase
```

Esse endpoint inicia o fluxo de compra do jogo e futuramente será responsável pela publicação do evento:

```text
OrderPlacedEvent
```

---

### **7. Estrutura da Solução**

```text
FCG.CatalogAPI
│
├── docs
├── src
│   ├── CatalogAPI
│   ├── CatalogAPI.Application
│   ├── CatalogAPI.Domain
│   ├── CatalogAPI.Infrastructure
│   └── CatalogAPI.Shared
│
├── tests
├── Dockerfile
├── docker-compose.yml
└── README.md
```

---

### **8. Finalização**

* Após seguir as etapas anteriores, o sistema será iniciado e a documentação Swagger estará disponível para utilização.

* Os endpoints poderão ser testados diretamente pela interface Swagger.

* Quando executado via Docker, a documentação estará disponível em:

```text
http://localhost:8080/swagger
```

---
