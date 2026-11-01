# SQL Server e Azure SQL

A aplicação usa exclusivamente MSSQL para persistência relacional. Dependências, consultas, locks, jobs, testes e migrações foram adaptados. Os dados do provedor anterior não foram convertidos; bancos e volumes existentes não foram apagados.

## Migrações

`src/Host/Recovery.BackOffice.Migrations.SqlServer` contém migrações e snapshots dos 11 contextos: Tenant, Identity, Audit, Eventing, Billing, Catalog, Tickets, Chat, Notifications, Files e Webhook. As factories de design não iniciam a API nem seus serviços externos.

O migrador utiliza `sp_getapplock` com propriedade de sessão. No Azure, o banco deve ser provisionado previamente; no SQL Server local, ele pode criar o banco de aplicação quando o usuário possui permissão. O destino deve ser um banco de aplicação, não `master`.

Configuração de cada processo por ambiente:

```text
DatabaseOptions__Provider=MSSQL
DatabaseOptions__MigrationsAssembly=Recovery.BackOffice.Migrations.SqlServer
DatabaseOptions__ConnectionString=<segredo de conexão do ambiente>
```

Após configurar a conexão de administração no console local:

```powershell
dotnet run --project src/Host/Recovery.BackOffice.DbMigrator -c Release -- apply
```

Para criar os dados iniciais, configurar também `Seed__DefaultAdminPassword` por segredo e utilizar `apply --seed`. `seed-demo` é exclusivo de Development e dados fictícios.

## Homologação Azure

Servidor provisionado pelo responsável: `recovery-backoffice-hml-sql.database.windows.net`; banco: `recovery-backoffice-hml`; grupo: `rg-recovery-backoffice-hml`. A oferta gratuita e a pausa ao esgotar a franquia foram verificadas no portal durante o provisionamento. A aplicação ainda não foi publicada nem as migrações aplicadas nesse banco por esta alteração.

A conexão Azure deve utilizar `Encrypt=True;TrustServerCertificate=False`, com credencial própria e firewall limitado aos clientes autorizados e IPs de saída do App Service. Senhas não devem constar em arquivos versionados ou argumentos compartilhados. A credencial de migração e a credencial de execução são configurações independentes dos dois processos.

O App Service existente é Windows F1, 32 bits e .NET 10. A publicação para esse destino precisa respeitar essa arquitetura. O bootstrap do Hangfire deve ser planejado com permissão DDL: o armazenamento inicializa seu schema e não deve receber permanentemente privilégios de administrador por conveniência.

Conexões, workers e verificações periódicas podem manter o banco ativo e consumir a franquia. Ao esgotá-la, a configuração escolhida pausa o banco; testes de jobs não equivalem a disponibilidade contínua nesse plano.

## Desenvolvimento

O AppHost provisiona SQL Server 2022 em Docker, mantendo cache e armazenamento locais. O Docker Compose também usa SQL Server Developer, destinado exclusivamente a desenvolvimento/testes. Os testes de integração usam bancos de aplicação próprios em containers SQL Server.

JSON é armazenado em `nvarchar(max)` com validação `ISJSON`; filtros estruturados de auditoria utilizam `JSON_VALUE`. O outbox usa claim atômico com `UPDLOCK`/`READPAST`; a realocação de arquivos usa lock de atualização. A chave da inbox preserva nomes curtos e representa nomes longos por SHA-256, evitando truncamento e colisões por prefixo.

Buscas comuns usam LIKE com collation explícita sem diferenciar maiúsculas/minúsculas. A busca do chat passa a ser por substring e não oferece operadores de busca textual avançada. Cursores UUID v7 usam ordenação textual canônica para evitar a ordenação específica de uniqueidentifier do SQL Server.

Os exemplos opcionais AWS preservam os módulos gerais, mas o banco é externo e a conexão vem de segredo existente. Não foram aplicados nem validados com Terraform nesta revisão.

## Validação

Builds Debug e Release com zero avisos/erros; 11 migrações e dados iniciais aplicados em SQL Server local vazio. Azure, pipeline remota e deployment permanecem pendentes.

Validação completa em 01/11/2026: nova execução integral dos 2.139 testes .NET (1.340 unitários/arquitetura e 799 de integração), todos aprovados e sem testes ignorados. Resultados em `artifacts/aspire-fix/backend`. Foram aprovados também 111 testes Playwright do Admin e 163 do Dashboard, com APIs simuladas: 2.413 testes distintos no total. Ambos os frontends compilaram; o lint terminou sem erros, com 19 avisos.

Execução local validada pelos comandos originais do README, sem alterações nos perfis de lançamento. O Aspire iniciou API e frontends; saúde da API, login de demonstração e as três URLs retornaram HTTP 200. A inicialização final não registrou erros nos recursos.

O provisionamento de bancos dedicados SQL Server usa as credenciais originais antes das migrações e limpa o cache de falha da conexão após criar o banco. Bancos Azure SQL continuam exigindo provisionamento prévio. Datas de faturamento preservam UTC ao serem materializadas de `datetime2`; transações explícitas nos testes de atomicidade são executadas dentro da estratégia de repetição do SQL Server.
