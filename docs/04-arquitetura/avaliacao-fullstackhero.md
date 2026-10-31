# Avaliação FullStackHero — projeto gerado e validação inicial

Data: 2026-10-31. Estado: candidato em avaliação; adoção não aprovada. Nenhum push realizado.

## Resultado

Atualização de execução local: AppHost, migração/seeder e API iniciados; admin/dashboard/API health responderam 200. Login demo acme pelo proxy React confirmado. Frontends iniciados diretamente com npm.cmd devido à falha de lançamento pelo Aspire no Windows; portas auxiliares ajustadas na cópia. Login root padrão retornou 401 e segue pendente, sem reset de credencial. Isso complementa a baseline de compilação abaixo; suíte completa/E2E permanece não executada. Pipeline anterior recuperado/adaptado para CI, sem deployment externo: [continuidade do pipeline](continuidade-pipeline.md).

Bom candidato para reaproveitar infraestrutura modular e acelerar uma prova de conceito. Não é implementação pronta das regras Recovery. A maior decisão é adaptar identidade/tenancy sem transformar escritório em tenant por conveniência: conta única entre escritórios e carteiras compartilhadas exigem modelo próprio.

Projeto: raiz do repositório. Upstream preservado em download local ignorado `.evaluation/fullstackhero-upstream`, commit `a967537526f3b769c7d268da3ed2a6920af65ce9`. Gerado por `dotnet new fsh`, não apenas renomeado manualmente, com Aspire e dois clientes React, sem guias de agentes. Licença MIT original copiada para a raiz gerada, pois o template a exclui na geração. Instruções em [guia de execução](guia-fullstackhero.md).

## Documentação e fonte consultadas

- [Repositório oficial](https://github.com/fullstackhero/dotnet-starter-kit).
- [Introdução e arquitetura](https://fullstackhero.net/docs/getting-started/introduction/).
- [Visão de arquitetura](https://fullstackhero.net/docs/architecture/).
- README, configuração de template, licença e arquivos de implementação da revisão local acima.

Documentação web pode evoluir independentemente do commit. O relatório diferencia conteúdo declarado, implementação inspecionada e execução observada; não reproduz alegações de “production-ready” como certificação.

## Validação executada

| Verificação | Resultado |
|---|---|
| SDK disponível | .NET 10.0.401; comando de informação completo teve restrição de sandbox, build executado com permissão apropriada |
| Node | 24.14.1 |
| Docker | Engine 29.7.2 acessível fora do sandbox; disponibilidade não comprova execução da pilha |
| Geração | Sucesso, com nome Recovery.TemplateEvaluation |
| Build backend original | Falhou em NuGetAudit, alertas NU1902/NU1903 em dependências transitivas |
| Build backend após patch local | Sucesso: 0 avisos e 0 erros |
| Identity.Tests | 340 aprovados, nenhum ignorado ou com falha |
| Architecture.Tests | 55 aprovados, nenhum ignorado ou com falha |
| Admin React | `npm ci` e `npm run build` concluídos |
| Dashboard React | `npm ci` e `npm run build` concluídos |

Builds React emitiram avisos de chunks acima de 500 kB e anotações de otimização SignalR; não impediram a geração. Não foi executada a suíte completa, testes de integração, E2E, análise completa de dependências npm nem login de ponta a ponta. A pilha Aspire não foi iniciada nesta avaliação. Não afirmar que requisitos Recovery passaram com base nos testes upstream.

## Patch local necessário

Em `src/Directory.Packages.props` da cópia gerada:

- Alinhar referências centrais de `10.0.12` para `10.0.14`.
- Atualizar pin de `System.Security.Cryptography.Xml` para `10.0.14`.
- Adicionar pins transitivos `System.Security.Cryptography.Pkcs` e `Microsoft.AspNetCore.SignalR.Common`, ambos `10.0.14`.

Primeiro ajuste isolado de criptografia revelou alerta de SignalR; pin de SignalR exigiu alinhar Microsoft.Extensions.Options e a linha de patches para evitar downgrade. Auditoria e warnings-as-errors permaneceram habilitados. Não houve alteração de BuildingBlocks. Isso comprova correção dos bloqueios observados no restore atual, não ausência universal de vulnerabilidades.

Avisos oficiais consultados: [Pkcs](https://github.com/advisories/GHSA-846f-f764-76mx) e [Xml](https://github.com/advisories/GHSA-7hqp-7c5m-4vq6). A versão corrigida do projeto é diferente da baseline upstream; preservar essa distinção ao comparar.

## Aderência às regras

| Área | Evidência local | Avaliação para Recovery |
|---|---|---|
| Modularidade | BuildingBlocks, módulos runtime + Contracts, NetArchTest | Boa base; fronteiras verificadas pelos 55 testes executados |
| APIs/dados | Minimal APIs, Mediator, FluentValidation, EF Core, migrador separado | Reaproveitável; ainda escolher banco e remover módulos sem uso |
| Login | `IdentityService` procura usuário por email | Adaptar para CPF e unicidade global |
| Identidade/tenancy | `IdentityDbContext` deriva de MultiTenantIdentityDbContext; tabelas Identity marcadas por tenant | Não equivale a conta única com múltiplos escritórios; exige desenho explícito |
| MFA | `IdentityService` verifica fator quando `user.TwoFactorEnabled` | Não satisfaz por si só obrigatoriedade universal e recuperação exclusiva Recovery |
| Sessões | `GenerateTokenCommandHandler` continua login se registro de sessão falhar | Inadequado como garantia de sessão única/revogação; tornar consistente com regras |
| Tokens frontend | `clients/admin/src/auth/token-store.ts` e equivalente dashboard persistem access/refresh em localStorage | Diverge da proposta BFF; decidir arquitetura antes de remotos arbitrários |
| Grupos/perfis | Entidades e serviços existem | Validar/adaptar hierarquia com múltiplos superiores, herança comum, cópia e limite delegável |
| Escritórios/carteiras | Modelo SaaS tenancy não demonstra nossas relações compartilhadas | Implementar acesso no contexto ativo sem presumir tenant=escritório |
| Portais/microfrontends | Dois apps Vite; isso não demonstra shell com catálogo de remotos | Construir composição, menus dinâmicos, disponibilidade backend e versionamento |
| Auditoria | Interceptor EF, publicador via canal, sink SQL e fallback em arquivo presentes | Avaliar atomicidade/durabilidade com alteração sensível; nomes não comprovam AUD-007 |
| Inativação/concorrência | Infraestrutura de persistência e testes disponíveis | Validar cada cadastro e importação contra critérios Recovery |
| Workflow | Módulos de exemplo Catalog/Tickets não especificam nosso processo | Não considerar atendido; recorte continua adiado |

## Infraestrutura trazida pelo template

AppHost orquestra PostgreSQL, Valkey, RustFS, ferramentas auxiliares, migrador, dados demo, API e dois clientes React. Há Hangfire, notificações, billing, chat, arquivos e webhooks. Esses recursos têm custos de operação e dependências; não precisam entrar todos na aplicação final. Terraform fornecido é direcionado a AWS, não plano Azure aprovado.

AppHost contém credenciais/dados de demonstração e configuração de email de teste. Não usar em produção nem promover como configuração corporativa. Não houve envio de dados de negócio ou credenciais Recovery a serviços externos nesta avaliação; nenhum serviço da aplicação foi iniciado.

## Critério de decisão proposto

Continuar a prova de conceito com três entregas pequenas:

1. Identidade global com CPF e dois escritórios, sem duplicar pessoa; escolher contexto e aplicar permissões distintas.
2. Sessão única + MFA obrigatório e revogação nos prazos acordados, com teste de concorrência.
3. Um remoto e uma funcionalidade com menu administrável; desativar e negar próxima requisição, preservando isolamento por portal.

Em paralelo, verificar auditoria durável na transação e retorno de versão de remoto. Só então estimar custo de adaptar versus aproveitar componentes seletivamente. Recomendação atual: aproveitar como base técnica candidata, sem assumir integralmente o modelo SaaS ou declarar adoção definitiva.
