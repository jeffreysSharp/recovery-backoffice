# Contexto do usuário

Adaptação dos contratos Core/Context, CustomException/ILocalizableMessage, CustomClaims, ClaimsPrincipalExtensions e CurrentUserService do FullStackHero. Namespaces usam Recovery BackOffice; a licença de origem está em licenses/fullstackhero.txt.

ICurrentUser expõe identidade, e-mail, tenant, roles e claims. ICurrentUserInitializer inicializa o principal ou um identificador de fallback. ICurrentUserService combina os dois contratos e é implementado internamente pelo módulo Identity.

Os três serviços apontam para a mesma instância Scoped. O estado de uma requisição/escopo não é compartilhado com outro. A inicialização de um principal já atribuído e a substituição de um fallback não vazio lançam CustomException, conforme o original. Guid.Empty e string vazia mantêm os comportamentos do modelo, sem regras adicionais de negócio.

Usuário autenticado obtém ID da claim NameIdentifier. Sem autenticação, GetUserId usa o fallback ou Guid.Empty; e-mail/tenant retornam string vazia. Um identificador autenticado inválido lança FormatException. A implementação não valida credenciais nem autentica alguém ao definir um ID de fallback.

CustomClaims mantém os nomes de claims originais. As extensões apenas leem os valores: não validam assinatura, expiração do token, vínculo com tenant ou autorização. IsInRole consulta o principal; não substitui a política de autorização do endpoint.

## Inicialização HTTP

CurrentUserMiddleware implementa IMiddleware e inicializa o serviço Scoped com HttpContext.User antes de chamar o próximo componente. IdentityModule registra o middleware e o adiciona em ConfigureMiddleware; o host executa esse ciclo por UseModuleMiddlewares. Cada requisição tem sua própria instância do contexto.

O middleware segue o modelo original e enriquece Activity.Current, quando existe e o principal está autenticado, com recovery.user_id, recovery.tenant_id e recovery.correlation_id. Não configura exportadores nem cria uma atividade artificial.

Ainda não há autenticação HTTP ou tokens. Na aplicação atual o principal normal é anônimo. Quando a autenticação for adicionada, UseAuthentication deve executar antes de UseModuleMiddlewares para que o principal correto seja capturado. O middleware não valida credenciais nem transforma headers em identidade.

Os testes HTTP usam principais sintéticos injetados apenas no host de testes, verificando o registro real do middleware e a separação entre requisições autenticadas e anônimas. O header X-Test-User-Id não é lido pelo código de produção.

CustomException mantém os detalhes e status do modelo, com contrato de localização. Isso não configura tratamento HTTP de exceções nem localização de mensagens. Os testes cobrem usuário autenticado/anônimo, inicialização, prioridade do principal, falhas de ID, alias Scoped, leitura das claims e preservação dos dados da exceção.
