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

## Modelo de Domínio

### Decisões de design
- **Profissional solo:** um login = um profissional. O campo `NomeExibicao` pode ser o nome pessoal ou da empresa — não há multi-usuário por conta.
- **Clientes por profissional:** cada profissional tem sua própria cartela de clientes (sem compartilhamento entre contas).
- **PIX manual:** o profissional cadastra sua chave PIX. O sistema gera o payload "Copia e Cola" e QR Code estático com o valor do serviço. Confirmação de pagamento é manual — o profissional marca como pago e pode anexar comprovante (imagem/PDF).

### Tabelas

| Tabela | Campos principais |
|---|---|
| `AspNetUsers` | Identity padrão (já existe) |
| `Categorias` | Id (int), Nome |
| `Profissionais` | Id, UserId (FK 1:1), CategoriaId (FK), NomeExibicao, Slug (unique), WhatsApp, Bio?, FotoUrl?, ChavePix?, TipoChavePix? (enum), Ativo, CriadoEm |
| `Servicos` | Id, ProfissionalId (FK), Nome, Descricao?, DuracaoMinutos, Preco, Ativo, CriadoEm |
| `HorariosDisponiveis` | Id, ProfissionalId (FK), DiaSemana (enum), HoraInicio, HoraFim, Ativo |
| `Bloqueios` | Id, ProfissionalId (FK), DataInicio, DataFim, Motivo? |
| `Clientes` | Id, ProfissionalId (FK), Nome, Telefone, Email?, Observacoes?, CriadoEm |
| `Agendamentos` | Id, ProfissionalId (FK), ServicoId (FK), ClienteId (FK), DataHoraInicio, DataHoraFim, Status (enum), ValorCobrado (snapshot), Observacoes?, CriadoEm |
| `Pagamentos` | Id, AgendamentoId (FK 1:1), StatusPagamento (enum), ComprovanteUrl?, DataConfirmacao?, CriadoEm |
| `NotificacoesAgendadas` | Id, AgendamentoId (FK), Tipo (enum), Status (enum), DataAgendada, DataEnvio?, Tentativas |
| `Planos` | Id (int), Nome, LimiteAgendamentosMensal (int? — null=ilimitado), Preco |
| `Assinaturas` | Id, ProfissionalId (FK), PlanoId (FK), Status (enum), DataInicio, DataFim?, CriadoEm |

**Enums relevantes:**
- `TipoChavePix`: CPF, CNPJ, Email, Telefone, Aleatoria
- `DiaSemana`: Segunda…Domingo
- `StatusAgendamento`: Pendente, Confirmado, Cancelado, Concluido
- `StatusPagamento`: Pendente, Pago, Isento
- `TipoNotificacao`: Confirmacao, Lembrete24h, Lembrete1h
- `StatusNotificacao`: Pendente, Enviado, Falhou
- `StatusAssinatura`: Ativa, Cancelada, Expirada

### Relações
```
AspNetUsers   1──1 Profissionais
Categorias    1──N Profissionais
Profissionais 1──N Servicos
Profissionais 1──N HorariosDisponiveis
Profissionais 1──N Bloqueios
Profissionais 1──N Clientes
Profissionais 1──N Agendamentos
Servicos      1──N Agendamentos
Clientes      1──N Agendamentos
Agendamentos  1──1 Pagamentos
Agendamentos  1──N NotificacoesAgendadas
Profissionais 1──N Assinaturas
Planos        1──N Assinaturas
```

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

## Fluxo Front → Back

Toda funcionalidade segue este fluxo obrigatório. Nunca pular camadas.

```
Blazor Page (.razor + .razor.cs)
  → Handler BFF  [Apresentacao/Handlers/<Feature>/]
    → HTTP (IHttpClientFactory, client "ApiBack")
      → IEndpoint  [Api/Endpoints/<Feature>/]
        → UseCase Handler  [Aplicacao/UseCases/<Feature>/<Action>/]
          → IRepository  [Dominio/Repositories/]
            → Repository impl  [Infraestrutura/Repositories/]
              → ApplicationDbContext (EF Core)
```

### Convenções por camada

**Blazor Page**
- Rota, rendermode e `@inherits PageBase` no `.razor`
- Toda lógica no code-behind `.razor.cs` herdando `ComponentBase`
- Sempre usar `when (ex is not NavigationException)` em catches que contenham `Nav.NavigateTo`
- `[CascadingParameter] Task<AuthenticationState>` para obter email do usuário logado

**Handler BFF** (`TaMarcado.Apresentacao/Handlers/<Feature>/`)
- Injeta `IHttpClientFactory`, usa client `"ApiBack"`
- Contém os models do formulário com DataAnnotations
- Retorna `Result { Success, Error }` ou `Result<T> { Success, Data, Error }`
- Nunca acessa banco diretamente

**IEndpoint** (`TaMarcado.Api/Endpoints/<Feature>/`)
- Implementa `IEndpoint` — registrado automaticamente via reflection
- Resolve `userId` via `UserManager.FindByEmailAsync` (o cookie só traz email)
- Resolve `professionalId` via `IProfessionalRepository.GetIdByUserIdAsync` quando necessário
- Chama o UseCase Handler e retorna `result.Match(onSuccess, onFailure)`

**UseCase** (`TaMarcado.Aplicacao/UseCases/<Feature>/<Action>/`)
- Três arquivos: `Command.cs` (record), `Handler.cs` (class), `Response.cs` (record)
- Namespace plural para evitar colisão com entidades: `UseCases.Professionals.*`, `UseCases.Services.*`
- Handler injeta apenas `IRepository` interfaces — nunca `ApplicationDbContext` diretamente
- Retorna `Result<TResponse>` do `TaMarcado.Compartilhado`
- Sempre envolve o corpo em try/catch retornando `Error.Problem` em caso de exceção

**IRepository** (`TaMarcado.Dominio/Repositories/`)
- Interface pura, sem dependência de infraestrutura
- Métodos retornam entidades do domínio ou tipos primitivos (nunca DTOs)

**Repository** (`TaMarcado.Infraestrutura/Repositories/`)
- Implementa a interface do domínio
- Injeta `ApplicationDbContext`
- Chama `SaveChangesAsync()` após operações de escrita

### Referências entre projetos
```
Apresentacao → (sem referência direta à API ou Domínio)
Api          → Aplicacao, Infraestrutura, Dominio, Compartilhado
Aplicacao    → Dominio, Compartilhado  (nunca Infraestrutura)
Infraestrutura → Dominio
Dominio      → DominioPrincipal
```

### Padrão Result
```csharp
// Sucesso
Result.Success(new MinhaResponse(...))
// Falha de negócio
Result.Failure<T>(Error.Conflict("Entidade.Codigo", "Mensagem"))
Result.Failure<T>(Error.NotFound("Entidade.Codigo", "Mensagem"))
// Exceção inesperada
Result.Failure<T>(Error.Problem("Entidade.Acao", ex.Message))
```

### Providers MudBlazor obrigatórios
`InteractiveShell.razor` deve conter `<MudSnackbarProvider />` e `<MudDialogProvider />`.
Sem o provider, Snackbar/Dialog não funcionam mesmo que o componente esteja no markup.

---

## Pontos importantes

- **Registro de serviços no `Program.cs`:** Todo `builder.Services.*` deve estar **antes** de `builder.Build()`. Colocar depois faz o container de DI ignorar o registro e quebra o EF em design time.

- **Migrations ficam em `TaMarcado.Infraestrutura`**, não na API. O `--startup-project` aponta para a API porque ela possui a connection string e o `AddDbContext`.

- **Configurações de entidade** são carregadas via `builder.ApplyConfigurationsFromAssembly(TaMarcadoAssemblyReference.Assembly)` no `OnModelCreating` do `ApplicationDbContext`. Novas `IEntityTypeConfiguration<T>` dentro de `TaMarcado.Infraestrutura` são registradas automaticamente.

- **Frontend:** O cliente Blazor WebAssembly usa **MudBlazor** como biblioteca de UI. O projeto server (`TaMarcado.Apresentacao`) hospeda o cliente WASM e é separado da API. O projeto de frontend jamais poderá acessar o banco de dados diretamente.

- **CORS:** A API libera `https://localhost:7137` (porta padrão do projeto Blazor server).


