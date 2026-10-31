# Carregamento dos módulos

O host informa explicitamente os assemblies em `builder.AddModules(...)`. Cada assembly declara seus módulos por `[assembly: RecoveryModule(typeof(IdentityModule), 1)]`. O atributo identifica a implementação e sua ordem; não registra serviços sozinho.

O ModuleLoader lê os atributos, remove duplicações por tipo e executa ConfigureServices antes de Build. A ordem é numérica, seguida do nome completo do tipo com comparação ordinal. Os mesmos módulos recebem ConfigureMiddleware e MapEndpoints após Build, nessa ordem de registro.

O padrão de atributo e ciclo segue o FullStackHero. A implementação é incremental: ainda não há Mediator, FluentValidation ou descoberta de todos os assemblies do processo. Os assemblies são explícitos para manter o conjunto de módulos previsível.

Diferentemente da lista estática global do carregador original, o registro pertence ao container de cada host. Isso permite inicializar vários hosts no mesmo processo sem compartilhar estado ou pular registros durante testes de integração. Uma segunda chamada de AddModules no mesmo builder falha; incluir todos os assemblies na primeira chamada.

Módulos precisam ser classes concretas com construtor público sem parâmetros. Serviços de negócio são registrados em ConfigureServices e resolvidos pelo container nos endpoints/handlers; não são dependências do construtor do módulo.

Os testes verificam descoberta por atributo, ordem, duplicações, ciclo de configuração, rotas, independência entre hosts e rejeição de registros inválidos. O teste HTTP do host continua verificando a composição real com Identity.
