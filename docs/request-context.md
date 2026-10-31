# Contexto HTTP da requisição

Segue a separação do FullStackHero: IRequestContext no Core, IRequestContextService nos contratos de Identity e RequestContextService interno na implementação de Identity. Os arquivos foram adaptados com namespaces Recovery BackOffice; a licença de origem permanece em licenses/fullstackhero.txt.

O serviço fornece IpAddress, UserAgent, ClientId e Origin. ClientId usa X-Client-Id ou o padrão web. Origin prefere OriginOptions:OriginUrl; sem configuração, usa scheme, host e path base da requisição. Sem ambos, retorna null. A origem representa a API e não deve ser usada como origem de links de recuperação de senha ou confirmação de e-mail do frontend.

IdentityModule registra IHttpContextAccessor, faz binding de OriginOptions e registra o serviço Scoped. IRequestContext e IRequestContextService apontam para a mesma instância do escopo. No modelo completo, o registro de accessor/opções faz parte da composição de plataforma; aqui está junto do módulo consumidor enquanto essa composição ainda não existe.

O Core permanece independente do ASP.NET Core. BuildingBlocks.Web contém OriginOptions e referencia Core; os contratos Identity referenciam Core. Não há endpoint diagnóstico expondo os metadados.

Headers e IP são metadados, não prova de identidade. Origin não é uma lista de origens confiáveis. Este incremento não configura proxies, autenticação, autorização, correlação ou resolução de tenant. ICurrentUser, claims, inicialização do usuário e middleware serão trazidos em incremento separado, seguindo suas dependências originais.

Os testes resolvem os serviços pelo container e verificam metadados HTTP, ausência de requisição, defaults, prioridade da origem configurada e lifetime Scoped. O teste HTTP existente continua verificando que o host real inicializa.
