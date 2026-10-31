# Contexto do usuário

Adaptação dos contratos Core/Context, CustomException/ILocalizableMessage, CustomClaims, ClaimsPrincipalExtensions e CurrentUserService do FullStackHero. Namespaces usam Recovery BackOffice; a licença de origem está em licenses/fullstackhero.txt.

ICurrentUser expõe identidade, e-mail, tenant, roles e claims. ICurrentUserInitializer inicializa o principal ou um identificador de fallback. ICurrentUserService combina os dois contratos e é implementado internamente pelo módulo Identity.

Os três serviços apontam para a mesma instância Scoped. O estado de uma requisição/escopo não é compartilhado com outro. A inicialização de um principal já atribuído e a substituição de um fallback não vazio lançam CustomException, conforme o original. Guid.Empty e string vazia mantêm os comportamentos do modelo, sem regras adicionais de negócio.

Usuário autenticado obtém ID da claim NameIdentifier. Sem autenticação, GetUserId usa o fallback ou Guid.Empty; e-mail/tenant retornam string vazia. Um identificador autenticado inválido lança FormatException. A implementação não valida credenciais nem autentica alguém ao definir um ID de fallback.

CustomClaims mantém os nomes de claims originais. As extensões apenas leem os valores: não validam assinatura, expiração do token, vínculo com tenant ou autorização. IsInRole consulta o principal; não substitui a política de autorização do endpoint.

Este incremento registra os serviços, mas não inclui CurrentUserMiddleware, autenticação HTTP ou publicação de tokens. O próximo passo será inicializar o serviço no pipeline HTTP na posição correta, após autenticação quando ela existir. Até lá, o host não preenche o contexto a partir de uma requisição automaticamente.

CustomException mantém os detalhes e status do modelo, com contrato de localização. Isso não configura tratamento HTTP de exceções nem localização de mensagens. Os testes cobrem usuário autenticado/anônimo, inicialização, prioridade do principal, falhas de ID, alias Scoped, leitura das claims e preservação dos dados da exceção.
