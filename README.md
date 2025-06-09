# Catálogo Interativo de Receitas Culinárias

Este projeto é uma aplicação full-stack que consiste em um backend **ASP.NET Core** para gerenciamento de receitas e um frontend em **Angular** para interação do usuário. A aplicação é totalmente containerizada com **Docker** para facilitar a implantação e o desenvolvimento.

## Funcionalidades

* **Backend:** API RESTful para operações CRUD (Criar, Ler, Atualizar, Deletar) de receitas.
* **Frontend:** Interface de usuário para visualizar, criar, editar e excluir receitas.
* **Banco de Dados:** Persistência de dados com **SQL Server**.
* **Containerização:** Suporte completo a **Docker** e **Docker Compose** para fácil configuração do ambiente.

## Pré-requisitos

### Para Desenvolvimento Local

* [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
* [Node.js e npm](https://nodejs.org/) (versão 20.x ou superior)
* [Angular CLI](https://angular.io/cli)
* [SQL Server](https://www.microsoft.com/pt-br/sql-server/sql-server-downloads) (ou SQL Server Express)

### Para Desenvolvimento com Docker

* [Docker Desktop](https://www.docker.com/products/docker-desktop/)

## Executando a Aplicação

Você pode executar a aplicação de duas maneiras: localmente em sua máquina ou utilizando Docker.

### 1. Executando Localmente

#### Backend (API)

1.  **Configure a String de Conexão:**
    * Abra o arquivo `ReceitasCulinarias.API/appsettings.json`.
    * Altere a `DefaultConnection` para apontar para sua instância local do SQL Server.

2.  **Aplique as Migrations:**
    * Abra um terminal na pasta raiz do projeto.
    * Execute o comando abaixo para aplicar as migrations do Entity Framework e criar o banco de dados:
        ```bash
        dotnet ef database update -p ReceitasCulinarias.Infrastructure -s ReceitasCulinarias.API
        ```

3.  **Inicie a API:**
    * Navegue até o diretório da API e inicie a aplicação:
        ```bash
        cd ReceitasCulinarias.API
        dotnet watch run
        ```
    * A API estará disponível em `http://localhost:5056` (verifique o `launchSettings.json` para a porta correta).

#### Frontend (Angular)

1.  **Instale as Dependências:**
    * Navegue até a pasta do frontend e instale os pacotes npm:
        ```bash
        cd frontend
        npm install
        ```

2.  **Inicie o Servidor de Desenvolvimento:**
    * Execute o comando para iniciar a aplicação Angular:
        ```bash
        ng serve
        ```
    * A aplicação estará acessível em `http://localhost:4200/`. As requisições para a API serão automaticamente redirecionadas.

---

### 2. Executando com Docker

O `docker-compose.yml` irá orquestrar todos os serviços necessários: a API, o frontend, o banco de dados e um proxy reverso NGINX.

1.  **Variáveis de Ambiente:**
    * Crie um arquivo `.env` na raiz do projeto (no mesmo nível do `docker-compose.yml`).
    * Adicione as seguintes variáveis de ambiente a este arquivo. Substitua os valores de exemplo por suas próprias chaves secretas:

        ```env
        # Senha para o usuário 'sa' do SQL Server
        DB_PASSWORD=SuaSenhaForteAqui#123

        # Configurações do JWT para a API (substitua por valores seguros)
        JWT_KEY=Chave_Secreta_Jwt_De_54_Caracteres_Projeto_Desafio_API
        JWT_ISSUER=DesafioAPI
        JWT_AUDIENCE=DesafioAPIClient
        ```

2.  **Inicie os Containers:**
    * Abra um terminal na raiz do projeto.
    * Execute o seguinte comando para construir as imagens e iniciar os containers:

        ```bash
        docker-compose up --build
        ```

3.  **Acesse a Aplicação:**
    * Após todos os serviços estarem em execução, a aplicação estará disponível em `http://localhost/`.
    * O **NGINX** atuará como um proxy reverso:
        * Requisições para `http://localhost/api/` serão direcionadas para o serviço da API.
        * Todas as outras requisições (`http://localhost/`) serão direcionadas para a aplicação Angular.

Para parar a aplicação, pressione `CTRL + C` no terminal onde o docker-compose está rodando, ou execute `docker-compose down` em outro terminal na mesma pasta.