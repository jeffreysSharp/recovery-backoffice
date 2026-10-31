# Persistência inicial do Identity

Esta etapa adapta FshUser, FshRole, FshRoleClaim, PasswordHistory, os cinco eventos usados pelo usuário, IdentityConfigurations, PasswordHistoryConfiguration e o contexto do FullStackHero. Os tipos da aplicação usam RecoveryUser, RecoveryRole e RecoveryRoleClaim; a licença permanece em licenses/fullstackhero.txt.

RecoveryUser preserva os campos e métodos do modelo, incluindo ativação, eventos, locale, refresh token e histórico de senhas. Os eventos ficam na entidade até ClearDomainEvents; ainda não existe dispatcher, outbox, envio de e-mail ou execução de handlers. PasswordHistory armazena hash, nunca a senha em texto. Ter a entidade não implementa ainda política de reutilização de senhas.

IdentityDbContext deriva de MultiTenantIdentityDbContext do Finbuckle 10.1.0, com os tipos customizados e suporte do ASP.NET Core Identity 10 para passkeys. A referência explícita a Microsoft.AspNetCore.Identity.EntityFrameworkCore 10.0.12 mantém o pacote alinhado ao EF Core usado pelo projeto. O contexto contém apenas o núcleo do Identity e o histórico de senhas: sessões, grupos e impersonação não foram importados.

Os mapeamentos do modelo usam o schema identity para Users, Roles, RoleClaims, UserClaims, UserRoles, UserLogins, UserTokens e PasswordHistory. Os limites de Locale e ObjectId são preservados. Depois das configurações, ApplyTenantIsolationByDefault protege entidades adicionais; as entidades Identity já marcadas são preservadas. Os índices únicos de nomes são escopados pelo tenant conforme o Finbuckle. Os eventos de domínio não viram tabelas.

O contexto preserva a conexão padrão quando o tenant não define outra. Para conexão dedicada, aplica a seleção do modelo e reutiliza o IScopedDbConnectionProvider já adotado no Recovery e no BaseDbContext. Essa é uma adaptação explícita ao contexto Identity do clone, que configura a string diretamente: no Recovery, contextos do mesmo escopo podem compartilhar a conexão e sua propriedade permanece no provider. A construção manual sem provider continua usando a string diretamente.

O IdentityModule ainda não registra o contexto nem UserManager, RoleManager ou stores do Identity. O host continua com seus serviços atuais e não passa a exigir banco por causa desta etapa. A próxima integração configurará os serviços do Identity, autenticação e catálogo; migrations e testes com dados reais permanecem pendentes.

Os testes cobrem comportamento do usuário, payloads dos eventos, independência da lista de papéis, geração de SQL Server, mapeamentos, isolamento de consultas com modelo em cache e seleção de conexão. Eles não criam tabelas, não validam credenciais nem comprovam isolamento de gravações em banco real.

## Registro dos serviços Identity

AddRecoveryIdentity concentra o trecho de registro do IdentityModule do FullStackHero: AddRecoveryDbContext<IdentityDbContext>, AddIdentity<RecoveryUser, RecoveryRole>, AddEntityFrameworkStores e AddDefaultTokenProviders. A extensão permite validar essa composição antes de chamá-la no módulo ativo. Não há flags para escolher uma implementação alternativa.

A política mantém senha de no mínimo 10 caracteres, com dígito, minúscula e maiúscula, sem obrigatoriedade de símbolo. Exige e-mail único e configura bloqueio de novas contas após 5 falhas por 15 minutos. O fluxo de login ainda precisará chamar os métodos do Identity que contabilizam falhas e consultam o bloqueio; configurar as opções não cria esse fluxo.

Os providers padrão atendem operações do Identity como confirmação de e-mail e recuperação de senha. Eles não implementam emissão de JWT nem a sessão do futuro BFF. AddIdentity também registra os esquemas de cookies padrão do ASP.NET Core; a integração futura terá de coordenar esses esquemas com a autenticação escolhida, como o módulo completo do modelo faz na configuração JWT.

Para usar a extensão, a composição precisa disponibilizar DatabaseOptions/IScopedDbConnectionProvider e IMultiTenantContextAccessor<AppTenantInfo>. Nesta etapa, IdentityModule e Program continuam sem chamá-la. O host publicado continua sem exigir banco; a ativação será feita com o catálogo, a autenticação e a configuração de persistência.

Os testes resolvem UserManager, RoleManager e stores EF por escopo e conferem os filtros nas consultas dos managers. Exercitam o validador de senha real, hashing e tokens de confirmação vinculados a usuário, propósito e security stamp. Somente os testes usam Data Protection efêmera em memória. A configuração de chaves do ambiente de produção será tratada na implantação. Ainda não há operações de criação, unicidade de e-mail ou bloqueio comprovadas em SQL Server real.
