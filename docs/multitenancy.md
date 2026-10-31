# Bases de multitenancy

AppTenantInfo, IAppTenantInfo, MultitenancyConstants e QuotaResource foram adaptados dos arquivos correspondentes de BuildingBlocks.Shared no FullStackHero. Namespaces usam Recovery BackOffice; o issuer constante de exemplo foi adaptado para recovery.backoffice. A licença original permanece em licenses/fullstackhero.txt.

AppTenantInfo herda de TenantInfo do Finbuckle.MultiTenant.Abstractions 10.1.0, como no clone de referência. Contém identificação, nome, conexão opcional por tenant, e-mail administrativo, ativação, validade, issuer, plano e limites de quotas. O enum QuotaResource é apenas dependência do contrato: não há contabilização, cobrança ou bloqueio por quotas.

O construtor administrativo preserva a validade inicial de um mês calculada por TimeProvider.System. AddValidity aplica AddMonths; SetValidity exige data posterior à atual. Activate e Deactivate rejeitam o tenant root. Foram mantidas as propriedades mutáveis e os comportamentos originais, sem adicionar regras próprias de negócio ou afirmar que o objeto valida todas as entradas.

Os testes usam datas fixas nos métodos de validade e verificam ativação, proteção de root, normalização da conexão e independência dos dicionários de quotas. A geração da validade inicial continua usando o relógio do modelo, sem alterar a assinatura do construtor.

Este incremento não registra o motor de resolução do Finbuckle, não lê headers, não resolve tenants, não cria catálogo nem provisiona empresas. Não existe ainda isolamento de consultas ou gravações. Ter a claim tenant no contexto do usuário não substitui resolução/autorização de tenant.

No modelo completo, a resolução HTTP pertence ao módulo Multitenancy e usa claims, header e query, além de stores de cache e EF Core. Essas dependências serão trazidas separadamente; não há uma classe genérica MultiTenancyOptions nessa base do modelo e não foi criada uma aqui.

## Isolamento padrão no modelo EF Core

TenantIsolationExtensions.ApplyTenantIsolationByDefault foi adaptado do BuildingBlocks.Persistence do FullStackHero, usando Finbuckle.MultiTenant.EntityFrameworkCore 10.1.0. Depois das configurações das entidades, a extensão marca entidades com chave como multitenant e inclui TenantId nos índices únicos. Entidades IGlobalEntity, tipos owned, tipos sem chave e entidades já marcadas explicitamente são preservados, como no modelo de referência.

A extensão deve ser chamada por um contexto derivado de MultiTenantDbContext, depois das configurações do modelo. O BaseDbContext agora centraliza essa chamada; o host ainda não usa um contexto multitenant. A resolução HTTP, o catálogo de tenants e a aplicação da regra aos módulos serão integrados nas próximas etapas.

Os testes verificam o SQL gerado pelo provider SQL Server para dois tenants com o mesmo modelo em cache, a ausência do filtro nas entidades globais, os índices únicos por tenant e as exceções de mapeamento. Não abrem conexão nem comprovam isolamento de dados gravados em um SQL Server real. IgnoreQueryFilters pode remover os filtros; resolver um tenant também não substitui autorização.

O host continua sem exigir conexão de banco. Nenhuma tabela, senha inicial ou recurso Azure foi criado.

## Contexto base dos módulos

Context/BaseDbContext, ModelBuilderExtensions e QueryFilters foram adaptados dos arquivos correspondentes do FullStackHero, preservando a licença em licenses/fullstackhero.txt. O BaseDbContext deriva de MultiTenantDbContext e combina o filtro nomeado SoftDelete com o isolamento por tenant. Contextos derivados devem configurar suas entidades e índices antes de chamar base.OnModelCreating.

Consultas comuns omitem ISoftDeletable.IsDeleted. IgnoreQueryFilters([QueryFilters.SoftDelete]) permite consultar registros excluídos mantendo o filtro de tenant; IgnoreQueryFilters() remove todos os filtros e exige cuidado nos futuros serviços. Entidades IGlobalEntity continuam sujeitas à exclusão lógica quando implementam ISoftDeletable.

Sem ConnectionString específica no tenant, o contexto mantém a conexão configurada em AddRecoveryDbContext. Quando há uma conexão específica, OnConfiguring a obtém do IScopedDbConnectionProvider do escopo, mantendo o compartilhamento entre contextos e a propriedade da conexão no provider. Na construção manual sem provider, usa a string de conexão diretamente, como no modelo.

A sobrecarga SaveChangesAsync(CancellationToken) preserva TenantNotSetMode.Overwrite do FullStackHero; o Finbuckle preenche o TenantId ausente e rejeita divergências com o tenant atual. A auditoria continua registrada por AddRecoveryDatabaseOptions/AddRecoveryDbContext e não foi duplicada no contexto base.

Os testes verificam composição dos filtros no SQL gerado, remoção seletiva de SoftDelete, escolha da conexão, compartilhamento por escopo, descarte e validação de TenantId no fluxo de SaveChangesAsync. As gravações são suprimidas por um interceptor de teste: não há servidor SQL, migrations ou teste de persistência real neste incremento.

## Catálogo de tenants

O projeto Recovery.BackOffice.Multitenancy inicia a persistência do módulo Multitenancy. Data/TenantDbContext deriva de EFCoreStoreDbContext<AppTenantInfo>, como no FullStackHero, e aplica as configurações do próprio assembly. Ele não deriva do BaseDbContext: o catálogo precisa localizar empresas antes de existir um tenant resolvido. O modelo atual contém apenas AppTenantInfo; provisionamento, temas e avisos de expiração serão adicionados quando necessários.

Data/Configurations/AppTenantInfoConfiguration mapeia tenant.Tenants e limita Plan a 64 caracteres. Id e o índice único de Identifier vêm do contexto de store do Finbuckle. A conversão JSON e o ValueComparer de QuotaLimits preservam o padrão do modelo. A adaptação para SQL Server troca jsonb por nvarchar(max), permitindo usar o conversor sem depender de um tipo JSON nativo do servidor.

Os testes geram o script de criação e o SQL de consulta com o provider SQL Server, verificam a unicidade no mapeamento e o round-trip das quotas. Também verificam que alterações no dicionário são detectadas pelo EF sem trocar sua instância, com snapshot independente do valor original.

Este incremento não registra o módulo no host nem o store de resolução. Não adiciona endpoints, contratos vazios, migrations, criação de empresas ou credenciais. A geração do script não executa DDL; a unicidade ainda precisa ser comprovada em um SQL Server real na etapa de integração com banco.
