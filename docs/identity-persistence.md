# Persistência inicial do Identity

Esta etapa adapta FshUser, FshRole, FshRoleClaim, PasswordHistory, os cinco eventos usados pelo usuário, IdentityConfigurations, PasswordHistoryConfiguration e o contexto do FullStackHero. Os tipos da aplicação usam RecoveryUser, RecoveryRole e RecoveryRoleClaim; a licença permanece em licenses/fullstackhero.txt.

RecoveryUser preserva os campos e métodos do modelo, incluindo ativação, eventos, locale, refresh token e histórico de senhas. Os eventos ficam na entidade até ClearDomainEvents; ainda não existe dispatcher, outbox, envio de e-mail ou execução de handlers. PasswordHistory armazena hash, nunca a senha em texto. Ter a entidade não implementa ainda política de reutilização de senhas.

IdentityDbContext deriva de MultiTenantIdentityDbContext do Finbuckle 10.1.0, com os tipos customizados e suporte do ASP.NET Core Identity 10 para passkeys. A referência explícita a Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.12 mantém o pacote alinhado ao EF Core usado pelo projeto. O contexto contém apenas o núcleo do Identity e o histórico de senhas: sessões, grupos e impersonação não foram importados.

Os mapeamentos do modelo usam o schema identity para Users, Roles, RoleClaims, UserClaims, UserRoles, UserLogins, UserTokens e PasswordHistory. Os limites de Locale e ObjectId são preservados. Depois das configurações, ApplyTenantIsolationByDefault protege entidades adicionais; as entidades Identity já marcadas são preservadas. Os índices únicos de nomes são escopados pelo tenant conforme o Finbuckle. Os eventos de domínio não viram tabelas.

O contexto preserva a conexão padrão quando o tenant não define outra. Para conexão dedicada, aplica a seleção do modelo e reutiliza o IScopedDbConnectionProvider já adotado no Recovery e no BaseDbContext. Essa é uma adaptação explícita ao contexto Identity do clone, que configura a string diretamente: no Recovery, contextos do mesmo escopo podem compartilhar a conexão e sua propriedade permanece no provider. A construção manual sem provider continua usando a string diretamente.

O IdentityModule ainda não registra o contexto nem UserManager, RoleManager ou stores do Identity. O host continua com seus serviços atuais e não passa a exigir banco por causa desta etapa. A próxima integração configurará os serviços do Identity, autenticação e catálogo; migrations e testes com dados reais permanecem pendentes.

Os testes cobrem comportamento do usuário, payloads dos eventos, independência da lista de papéis, geração de SQL Server, mapeamentos, isolamento de consultas com modelo em cache e seleção de conexão. Eles não criam tabelas, não validam credenciais nem comprovam isolamento de gravações em banco real.
