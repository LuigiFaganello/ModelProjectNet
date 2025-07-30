# ModelProjectNet

Este projeto é uma aplicação .NET estruturada seguindo os princípios da Arquitetura Limpa (Clean Architecture). O objetivo é proporcionar uma clara separação de responsabilidades, tornando a base de código mais fácil de manter, testar e escalar.

## Sumário

1.  [Visão Geral da Arquitetura](#visão-geral-da-arquitetura)
2.  [Estrutura do Projeto](#estrutura-do-projeto)
    *   [src/](#src)
        *   [Application/](#application)
        *   [Domain/](#domain)
        *   [Infrastructure/](#infrastructure)
        *   [Web.Api/](#webapi)
        *   [WorkerService/](#workerservice)
    *   [tests/](#tests)
        *   [UnitTests/](#unittests)
3.  [Tecnologias e Bibliotecas](#tecnologias-e-bibliotecas)
    *   [Camada de Aplicação](#camada-de-aplicação)
    *   [Camada de Domínio](#camada-de-domínio)
    *   [Camada de Infraestrutura](#camada-de-infraestrutura)
    *   [Camada Web.Api](#camada-webapi)
    *   [Camada WorkerService](#camada-workerservice)
    *   [Projeto UnitTests](#projeto-unittests)

## Visão Geral da Arquitetura

Este projeto adere aos princípios da Arquitetura Limpa, que promovem uma abordagem em camadas para o design de software. A ideia central é manter as regras de negócio independentes de frameworks, bancos de dados e serviços externos.

As principais camadas são:

*   **Domínio (Entidades e Regras de Negócio):** A camada mais interna, contendo a lógica de negócio central e as entidades. Não possui dependências de outras camadas.
*   **Aplicação (Casos de Uso e Serviços de Aplicação):** Esta camada orquestra o fluxo de dados de e para a camada de Domínio. Ela define os casos de uso da aplicação e depende apenas da camada de Domínio.
*   **Infraestrutura (Preocupações Externas):** Esta camada lida com todas as interações externas, como acesso a banco de dados, APIs externas e implementações específicas de frameworks. Ela depende da camada de Domínio e implementa interfaces definidas nas camadas de Domínio e Aplicação.
*   **Apresentação (Web.Api e WorkerService):** Estas são as camadas mais externas, responsáveis por apresentar dados ao usuário ou executar tarefas em segundo plano. Elas dependem das camadas de Aplicação e Infraestrutura.

Esta estrutura em camadas garante que as mudanças em preocupações externas (por exemplo, a troca de bancos de dados) não afetem a lógica de negócio central.

![Diagrama](/docs/Desenho-de-arquitetura.svg)

## Estrutura do Projeto

O projeto é organizado em várias pastas, cada uma representando uma camada ou preocupação distinta dentro da Arquitetura Limpa.

### `src/`

Este diretório contém o código-fonte principal da aplicação, dividido em diferentes projetos que representam as camadas arquiteturais.

#### `Application/`

Este projeto contém as regras de negócio específicas da aplicação e os casos de uso. Ele define os serviços da aplicação e os objetos de transferência de dados (DTOs).

*   **`Application.csproj`**: Arquivo de projeto para a camada de Aplicação.
*   **`DependencyInjection.cs`**: Lida com a injeção de dependência para a camada de Aplicação.
*   **`Common/`**: Contém componentes comuns de nível de aplicação.
    *   **`ApplicationException.cs`**: Exceção personalizada para erros de nível de aplicação.
    *   **`Exceptions/`**: Diretório para outras exceções específicas da aplicação.
    *   **`Mappings/`**: Contém configurações de mapeamento (por exemplo, perfis do AutoMapper).
*   **`DTO/`**: Objetos de Transferência de Dados usados para comunicação entre as camadas de aplicação e apresentação.
*   **`Interfaces/`**: Interfaces para serviços de aplicação.
*   **`Services/`**: Implementações de serviços de aplicação, orquestrando a lógica de negócio.

#### `Domain/`

Esta é a camada central da aplicação, contendo as regras de negócio e entidades de toda a empresa. É independente de todas as outras camadas.

*   **`Domain.csproj`**: Arquivo de projeto para a camada de Domínio.
*   **`Common/`**: Contém componentes comuns de nível de domínio.
    *   **`Error.cs`**: Representa erros específicos do domínio.
    *   **`Result.cs`**: Um tipo de resultado genérico para operações que podem ter sucesso ou falhar, encapsulando valores de sucesso ou erros.
*   **`Entities/`**: Define as entidades de negócio centrais.
    *   **`EntityBase.cs`**: Classe base para entidades de domínio.
    *   **`Example.cs`**: Uma entidade de domínio de exemplo.
*   **`Exceptions/`**: Exceções personalizadas para erros específicos do domínio.
    *   **`DomainException.cs`**: Exceção personalizada para erros de nível de domínio.
*   **`Interfaces/`**: Interfaces para repositórios e outros serviços de domínio.
    *   **`IExampleRepository.cs`**: Interface para o repositório da entidade de exemplo.
    *   **`IRepositoryBase.cs`**: Interface base para repositórios.

#### `Infrastructure/`

Este projeto é responsável por implementar as interfaces definidas nas camadas de Domínio e Aplicação. Ele lida com preocupações externas, como acesso a banco de dados, integrações de API externas e detalhes específicos do framework.

*   **`Infrastructure.csproj`**: Arquivo de projeto para a camada de Infraestrutura.
*   **`DependencyInjection.cs`**: Lida com a injeção de dependência para a camada de Infraestrutura.
*   **`Configuration/`**: Classes de configuração para serviços externos e configurações da aplicação.
    *   **`AppSettings.cs`**: Configurações de toda a aplicação.
    *   **`ExampleEntityConfiguration.cs`**: Configuração do Entity Framework Core para a entidade `Example`.
*   **`Context/`**: Contexto de banco de dados para o Entity Framework Core.
    *   **`DataContext.cs`**: O contexto principal do banco de dados.
    *   **`DataContextFactory.cs`**: Fábrica para criar instâncias de `DataContext` (útil para migrações).
*   **`Extensions/`**: Métodos de extensão para várias funcionalidades.
    *   **`HttpClientExtensions.cs`**: Métodos de extensão para `HttpClient`.
*   **`ExternalService/`**: Integrações com APIs externas.
    *   **`ExampleService.cs`**: Implementação para um serviço externo.
    *   **`DTO/`**: Objetos de Transferência de Dados para comunicação de serviço externo.
    *   **`Interface/`**: Interfaces para serviços externos.
*   **`Migrations/`**: Arquivos de migração de banco de dados do Entity Framework Core.
*   **`Repositories/`**: Implementações de interfaces de repositório definidas na camada de Domínio.
    *   **`ExampleRepository.cs`**: Implementação de `IExampleRepository`.
    *   **`RepositoryBase.cs`**: Implementação base para repositórios.

#### `Web.Api/`

Este projeto é a camada de apresentação da API RESTful da aplicação. Ele expõe endpoints para que as aplicações cliente interajam com o sistema.

*   **`Web.Api.csproj`**: Arquivo de projeto para a camada Web.Api.
*   **`appsettings.json`**: Configurações da aplicação.
*   **`appsettings.Development.json`**: Configurações da aplicação específicas para o ambiente de desenvolvimento.
*   **`Dockerfile`**: Dockerfile para conteinerizar a API.
*   **`Program.cs`**: Ponto de entrada da aplicação Web API.
*   **`Configurations/`**: Configuração para vários aspectos da API.
    *   **`CorsConfiguration.cs`**: Configuração de Cross-Origin Resource Sharing (CORS).
    *   **`HealthcheckConfiguration.cs`**: Configurações de verificação de saúde.
    *   **`Swagger/`**: Configuração de documentação Swagger/OpenAPI.
*   **`Controllers/`**: Controladores de API que lidam com requisições HTTP de entrada.
    *   **`BaseController.cs`**: Controlador base para funcionalidades comuns.
    *   **`V1/`**: Controladores de API da Versão 1.
    *   **`V2/`**: Controladores de API da Versão 2.
*   **`Markdown/`**: Arquivos Markdown para documentação da API (por exemplo, para a UI do Swagger).
*   **`Middleware/`**: Middlewares personalizados para processamento de requisições.
    *   **`CorrelationMiddleware.cs`**: Middleware para adicionar IDs de correlação às requisições.
    *   **`GlobalExceptionMiddleware.cs`**: Middleware para tratamento global de exceções.
*   **`Properties/`**: Propriedades específicas do projeto.
    *   **`launchSettings.json`**: Perfis de depuração e inicialização.
*   **`wwwroot/`**: Arquivos estáticos servidos pela aplicação web (por exemplo, ativos personalizados da UI do Swagger).

#### `WorkerService/`

Este projeto é um serviço em segundo plano que pode executar tarefas agendadas ou operações de longa duração.

*   **`WorkerService.csproj`**: Arquivo de projeto para o WorkerService.
*   **`appsettings.json`**: Configurações da aplicação para o serviço de worker.
*   **`Dockerfile`**: Dockerfile para conteinerizar o serviço de worker.
*   **`Program.cs`**: Ponto de entrada da aplicação Worker Service.
*   **`Configuration/`**: Configuração para o serviço de worker.
    *   **`QuartzConfiguration.cs`**: Configuração para o agendador Quartz.NET.
*   **`Jobs/`**: Implementações de jobs em segundo plano.
    *   **`ExampleJob.cs`**: Um job em segundo plano de exemplo.
    *   **`ExampleSecondJob.cs`**: Outro job em segundo plano de exemplo.
*   **`Properties/`**: Propriedades específicas do projeto.
    *   **`launchSettings.json`**: Perfis de depuração e inicialização.

### `tests/`

Este diretório contém todos os projetos de teste para a aplicação.

#### `UnitTests/`

Este projeto contém testes de unidade para as várias camadas da aplicação, garantindo que os componentes individuais funcionem como esperado.

*   **`UnitTests.csproj`**: Arquivo de projeto para os UnitTests.
*   **`GlobalUsings.cs`**: Diretivas de uso globais para testes.
*   **`Application/`**: Testes de unidade para a camada de Aplicação.
*   **`Infrastructure/`**: Testes de unidade para a camada de Infraestrutura.
*   **`Web.Api/`**: Testes de unidade para a camada Web.Api.

## Tecnologias e Bibliotecas

Esta seção lista as principais tecnologias e pacotes NuGet usados em cada projeto.

- .Net Core 9
- MySql
- Docker

### Camada de Aplicação

*   **`Microsoft.Extensions.Logging.Abstractions`**: Fornece abstrações para logging.

### Camada de Domínio

*   Nenhum pacote NuGet externo referenciado diretamente, enfatizando sua independência.

### Camada de Infraestrutura

*   **`Microsoft.AspNetCore.Http.Abstractions`**: Abstrações para contexto HTTP.
*   **`Microsoft.EntityFrameworkCore.Design`**: Ferramentas de design-time para Entity Framework Core (por exemplo, migrações).
*   **`Microsoft.Extensions.Configuration`**: Abstrações de configuração.
*   **`Microsoft.Extensions.Configuration.FileExtensions`**: Provedor de configuração baseado em arquivo.
*   **`Microsoft.Extensions.Configuration.Json`**: Provedor de configuração JSON.
*   **`Microsoft.Extensions.Http`**: Integra `HttpClient` com injeção de dependência.
*   **`Newtonsoft.Json`**: Biblioteca popular de serialização/desserialização JSON.
*   **`Pomelo.EntityFrameworkCore.MySql`**: Provedor MySQL para Entity Framework Core.

### Camada Web.Api

*   **`Microsoft.AspNetCore.OpenApi`**: Ferramentas para gerar especificações OpenAPI.
*   **`Serilog.AspNetCore`**: Integração do Serilog com ASP.NET Core.
*   **`Serilog.Sinks.Seq`**: Sink do Serilog para Seq (servidor de log estruturado).
*   **`Serilog`**: Biblioteca de logging Serilog principal.
*   **`Swashbuckle.AspNetCore`**: Gera documentação Swagger/OpenAPI para APIs ASP.NET Core.
*   **`Swashbuckle.AspNetCore.Annotations`**: Fornece atributos para aprimorar a documentação Swagger.
*   **`Asp.Versioning.Mvc`**: Versionamento de API para ASP.NET Core MVC.
*   **`Asp.Versioning.Mvc.ApiExplorer`**: Integração do API Explorer para versionamento de API.

### Camada WorkerService

*   **`Microsoft.Extensions.Hosting`**: Fornece abstrações para hospedar serviços em segundo plano.
*   **`Quartz`**: Uma biblioteca de agendamento robusta para .NET.
*   **`Quartz.Extensions.Hosting`**: Integra o Quartz.NET com `Microsoft.Extensions.Hosting`.

### Projeto UnitTests

*   **`coverlet.collector`**: Ferramenta de cobertura de código.
*   **`Microsoft.NET.Test.Sdk`**: SDK para projetos de teste .NET.
*   **`xunit`**: Uma ferramenta de teste de unidade gratuita, de código aberto e focada na comunidade para .NET.
*   **`xunit.runner.visualstudio`**: Executor de testes para xUnit no Visual Studio.
*   **`FluentAssertions`**: Uma biblioteca .NET que permite afirmar fluentemente o resultado de testes de unidade.
*   **`Microsoft.EntityFrameworkCore.InMemory`**: Provedor de banco de dados em memória para Entity Framework Core, útil para testes.
*   **`Moq`**: Uma biblioteca de mocking para .NET.