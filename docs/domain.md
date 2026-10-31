# Base de domínio

Os arquivos de Core/Domain foram adaptados do FullStackHero em C:/Projects/FullStackHero/src/BuildingBlocks/Core/Domain. Foram preservados os contratos e comportamentos do modelo, com o namespace Recovery.BackOffice.BuildingBlocks.Core.Domain. O aviso de copyright e a licença original estão em licenses/fullstackhero.txt.

BaseEntity<TId> mantém o identificador com setter protegido e acumula eventos para processamento posterior. AggregateRoot<TId> herda dessa base; ainda não acrescenta comportamento. O identificador da entidade não é gerado automaticamente.

IDomainEvent herda de Mediator.INotification, como no modelo. DomainEvent contém EventId, OccurredOnUtc, CorrelationId e TenantId. A fábrica Create gera um Guid e um timestamp UTC, passando-os à função fornecida. O projeto referencia somente Mediator.Abstractions 3.0.2; não há dispatcher ou publicação de eventos configurados no host.

IHasDomainEvents permite consultar e limpar os eventos pendentes. Limpar os eventos não altera a identidade ou outros dados da entidade. As listas pertencem a cada instância, sem estado global.

IAuditableEntity, ISoftDeletable e IHasTenant definem contratos; ainda não preenchem auditoria, aplicam filtros ou resolvem tenants. IGlobalEntity será a exceção explícita ao isolamento quando a persistência multitenant for incorporada. Não usar o marcador como autorização para consultas globais neste estágio.

Os testes verificam ordem e isolamento dos eventos, limpeza, reutilização da coleção e a fábrica de eventos. Não foram criados testes para a mera existência de interfaces. Este incremento não altera o host nem conecta a base ao EF Core.
