# Bases de multitenancy

AppTenantInfo, IAppTenantInfo, MultitenancyConstants e QuotaResource foram adaptados dos arquivos correspondentes de BuildingBlocks.Shared no FullStackHero. Namespaces usam Recovery BackOffice; o issuer constante de exemplo foi adaptado para recovery.backoffice. A licença original permanece em licenses/fullstackhero.txt.

AppTenantInfo herda de TenantInfo do Finbuckle.MultiTenant.Abstractions 10.1.0, como no clone de referência. Contém identificação, nome, conexão opcional por tenant, e-mail administrativo, ativação, validade, issuer, plano e limites de quotas. O enum QuotaResource é apenas dependência do contrato: não há contabilização, cobrança ou bloqueio por quotas.

O construtor administrativo preserva a validade inicial de um mês calculada por TimeProvider.System. AddValidity aplica AddMonths; SetValidity exige data posterior à atual. Activate e Deactivate rejeitam o tenant root. Foram mantidas as propriedades mutáveis e os comportamentos originais, sem adicionar regras próprias de negócio ou afirmar que o objeto valida todas as entradas.

Os testes usam datas fixas nos métodos de validade e verificam ativação, proteção de root, normalização da conexão e independência dos dicionários de quotas. A geração da validade inicial continua usando o relógio do modelo, sem alterar a assinatura do construtor.

Este incremento não registra o motor de resolução do Finbuckle, não lê headers, não resolve tenants, não cria catálogo nem provisiona empresas. Não existe ainda isolamento de consultas ou gravações. Ter a claim tenant no contexto do usuário não substitui resolução/autorização de tenant.

No modelo completo, a resolução HTTP pertence ao módulo Multitenancy e usa claims, header e query, além de stores de cache e EF Core. Essas dependências serão trazidas separadamente; não há uma classe genérica MultiTenancyOptions nessa base do modelo e não foi criada uma aqui.

O próximo incremento de persistência avaliará os filtros e o BaseDbContext do modelo, incluindo Finbuckle.MultiTenant.EntityFrameworkCore. O host continua sem exigir conexão de banco. Nenhuma tabela, senha inicial ou recurso Azure foi criado.
