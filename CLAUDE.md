# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Visão Geral
TaMarcado é um SaaS de agendamento online voltado para profissionais autônomos brasileiros
(ex: barbeiros, psicólogos, personal trainers, dentistas, tatuadores, etc).
O objetivo é permitir que clientes agendem horários de forma simples, com confirmações
automáticas via WhatsApp, reduzindo faltas e eliminando a gestão manual de agenda.

## Modelo de Negócio
- Assinatura mensal por profissional/empresa
- Faixa de preço estimada: R$ 14,99
- Possível plano freemium com limite de agendamentos

## Funcionalidades Core
- Agendamento online via link personalizado
- Confirmação e lembretes automáticos via WhatsApp
- Controle de horários disponíveis
- Painel de agenda do profissional
- Pagamento via PIX integrado ao agendamento

## Commands

```bash
# Rodar a API e frontend Blazor
dotnet watch run

# Build de tudo
dotnet build

# Adicionar migration
dotnet ef migrations add <NomeDaMigration> --project TaMarcado.Infraestrutura/ --startup-project TaMarcado.Api/

# Aplicar migrations no banco
dotnet ef database update --project TaMarcado.Infraestrutura/ --startup-project TaMarcado.Api/

# Subir o banco de dados (Docker)
docker compose up -d
```

## Infraestrutura local

O SQL Server roda via Docker na porta `14433` (mapeada para a `1433` interna). A API e o frontend rodam localmente, fora do Docker.

Connection string (em `TaMarcado.Api/appsettings.json`):
```
Server=127.0.0.1,14433;Database=TaMarcadoDb;User Id=sa;Password=SqlServer123!;TrustServerCertificate=True;
```

## Arquitetura

Clean Architecture separado em camadas com os seguintes projetos:

| Projeto | Responsabilidade |
|---|---|
| `TaMarcado.Api` | Web API ASP.NET Core 10 — ponto de entrada, configuração de DI, endpoints do Identity |
| `TaMarcado.Infraestrutura` | `ApplicationDbContext`, migrations EF Core, `ApplicationUser` |
| `TaMarcado.Dominio` | Entidades e regras de domínio |
| `TaMarcado.Aplicacao` | Casos de uso / serviços de aplicação |
| `TaMarcado.Compartilhado` | Tipos e utilitários compartilhados entre camadas |
| `TaMarcado.Apresentacao` | Blazor Server (host, net10) + Blazor WebAssembly Client (net9, MudBlazor) |

## Pontos importantes

- **Registro de serviços no `Program.cs`:** Todo `builder.Services.*` deve estar **antes** de `builder.Build()`. Colocar depois faz o container de DI ignorar o registro e quebra o EF em design time.

- **Migrations ficam em `TaMarcado.Infraestrutura`**, não na API. O `--startup-project` aponta para a API porque ela possui a connection string e o `AddDbContext`.

- **Configurações de entidade** são carregadas via `builder.ApplyConfigurationsFromAssembly(TaMarcadoAssemblyReference.Assembly)` no `OnModelCreating` do `ApplicationDbContext`. Novas `IEntityTypeConfiguration<T>` dentro de `TaMarcado.Infraestrutura` são registradas automaticamente.

- **Frontend:** O cliente Blazor WebAssembly usa **MudBlazor** como biblioteca de UI. O projeto server (`TaMarcado.Apresentacao`) hospeda o cliente WASM e é separado da API. O projeto de frontend jamais poderá acessar o banco de dados diretamente.

- **CORS:** A API libera `https://localhost:7137` (porta padrão do projeto Blazor server).


