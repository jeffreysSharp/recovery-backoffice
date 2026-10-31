# Configuração de persistência

Este incremento adapta a organização Shared/Persistence do modelo FullStackHero para Recovery BackOffice e SQL Server. Ainda não registra banco no host, abre conexões, cria entidades, aplica migrações ou implementa isolamento por tenant.

## Projetos e responsabilidades

- BuildingBlocks.Shared/Persistence: DatabaseOptions e DbProviders. O único provedor deste projeto é MSSQL; é também o padrão.
- BuildingBlocks.Persistence: registro das opções com AddRecoveryDatabaseOptions e configuração do EF Core com ConfigureRecoveryDatabase.
- Tests/Unit/BuildingBlocks/Persistence: validação das opções, binding, falha na inicialização e inspeção da configuração SQL Server sem acesso à rede/banco.

DatabaseOptions mantém os campos Provider, ConnectionString e MigrationsAssembly do modelo. A validação rejeita connection strings vazias ou compostas apenas de espaços. Isso não comprova sintaxe, credenciais ou conectividade. O registro de opções rejeita provedor vazio; o configurador EF rejeita provedores não suportados.

O configurador usa UseSqlServer, define o assembly de migrações e habilita retry de falhas transitórias. Em Development habilita diagnósticos detalhados e sensitive data logging, como no modelo. Não habilitar essa opção em produção nem compartilhar logs locais com valores sensíveis.

O pacote System.Security.Cryptography.Pkcs é referenciado explicitamente na versão 10.0.13 para corrigir a versão transitiva 9.0.11 trazida pelo provedor SQL Server. Os advisories GHSA-2fmv-rv2c-59x7 e GHSA-846f-f764-76mx indicam essa versão como corrigida. A auditoria NuGet permanece ativa.

Não foi copiada a redução de PendingModelChangesWarning para log: preservamos o comportamento padrão do EF, sem enfraquecer a detecção de inconsistências nas migrações.

O host atual não referencia nem chama esses registros. A próxima integração exigirá uma connection string real em configuração externa; nunca versionar credenciais. Não foi criado banco ou recurso Azure.

## Conexões por escopo

AddRecoveryDatabaseOptions registra IScopedDbConnectionProvider com TryAddScoped, seguindo o modelo original. ScopedDbConnectionProvider mantém uma SqlConnection por connection string exata dentro do escopo, sem abrir conexão. Escopos diferentes possuem objetos independentes. A classe não é um cache global nem suporta uso concorrente do mesmo escopo.

A sobrecarga ConfigureRecoveryDatabase que recebe DbConnection informa contextOwnsConnection: false. Descartar um DbContext não descarta a conexão compartilhada; o escopo é o proprietário e realiza o descarte síncrono ou assíncrono. O provider rejeita uso após descarte e provedores não suportados, inclusive quando a string já está em cache.

O compartilhamento prepara transações entre contextos, mas não inicia nem coordena transações. SQL Server, esquema, credenciais e operações transacionais reais continuam fora deste incremento. O host ainda não ativa persistência.

## Registro dos contextos

AddRecoveryDbContext<TContext> segue o registro genérico do modelo: obtém IHostEnvironment, DatabaseOptions e IScopedDbConnectionProvider pelo container e configura o contexto com a conexão do escopo. O contexto e suas opções têm lifetime Scoped, padrão de AddDbContext.

Registrar AddRecoveryDatabaseOptions antes de resolver os contextos. Contextos devem receber DbContextOptions<TContext> no construtor. A configuração é aplicada ao resolver as opções; esse registro não abre conexão nem executa migrações.

Esta etapa ainda não adiciona os interceptores e o registro de transações presentes no método completo do FullStackHero: suas dependências serão incorporadas nos próximos incrementos. O host continua sem registrar um contexto de negócio.

Os testes verificam dois tipos de contexto compartilhando a conexão do mesmo escopo, separação entre escopos, diagnósticos por ambiente, falha de configuração e descarte da conexão no encerramento do escopo. São testes do registro e composição, sem SQL real.

## Auditoria de alterações

AuditableEntitySaveChangesInterceptor foi adaptado de Persistence/Inteceptors no FullStackHero; o namespace local usa a grafia Interceptors. Sua licença de origem está em licenses/fullstackhero.txt. O interceptor é registrado como ISaveChangesInterceptor Scoped e AddRecoveryDbContext incorpora os interceptores registrados. TimeProvider.System é o relógio padrão, substituível pelo container.

IAuditableEntity recebe CreatedOnUtc/CreatedBy ao adicionar e LastModifiedOnUtc/LastModifiedBy ao alterar, inclusive quando apenas uma referência owned mudou. ISoftDeletable converte Deleted em Modified, preenchendo os campos de exclusão e preservando referências owned que o EF marcou para exclusão em cascata.

O usuário de auditoria só é preenchido quando ICurrentUser.IsAuthenticated é true; um ID de fallback sem autenticação não é usado, como no modelo. A aplicação consumidora deve registrar ICurrentUser antes de resolver um contexto com auditoria. Os testes de composição usam um usuário anônimo de teste; o host de produção já possui esse serviço pelo Identity, mas ainda não registra contextos de negócio.

Os testes usam relógio fixo e o change tracker SQL Server, com um segundo interceptor suprimindo a gravação final. Não abrem conexão. Cobrem execução síncrona/assíncrona, criação, alteração, referências owned, soft delete e entidades sem o contrato. Não comprovam SQL ou persistência real. Filtros de consulta para ocultar itens excluídos serão implementados com BaseDbContext; o interceptor sozinho não oculta registros.

## Partes posteriores do modelo

Interceptor de eventos, registro de transações, BaseDbContext multitenant e migrações entram quando suas dependências forem trazidas e testadas. Não substituir Finbuckle por um modelo de tenants próprio neste incremento.

Os testes atuais validam configuração; testes de SQL e isolamento exigirão SQL Server real em um incremento posterior. Executar scripts/verify.ps1 antes de qualquer commit.
