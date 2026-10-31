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

## Partes posteriores do modelo

AddRecoveryDbContext, conexões por escopo, interceptores de auditoria/eventos, BaseDbContext multitenant e migrações entram quando suas dependências forem trazidas e testadas. Não substituir Finbuckle por um modelo de tenants próprio neste incremento.

Os testes atuais validam configuração; testes de SQL e isolamento exigirão SQL Server real em um incremento posterior. Executar scripts/verify.ps1 antes de qualquer commit.
