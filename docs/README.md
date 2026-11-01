# Documentação do MVP Recovery Backoffice

Início: 2026-10-31. Estado: regras consolidadas após entrevista E02–E38; arquitetura a iniciar.

## Referência atual

- [Personalização Recovery BackOffice](04-arquitetura/personalizacao-recovery.md): nomes atuais, compatibilidade e validação.

- [Continuidade do pipeline/Sonar](04-arquitetura/continuidade-pipeline.md): CI adaptada e dependências da implantação.

- [Avaliação](04-arquitetura/avaliacao-template.md): projeto gerado, builds/testes e diferenças para Recovery.

- [Proposta arquitetural .NET/React](04-arquitetura/proposta-arquitetural.md): comparação, módulos, padrões, pastas e decisões abertas.

- [Regras vigentes](regras-vigentes.md): estado consolidado aprovado/provisório, sem sequência de hipóteses substituídas.
- [Pendências atuais](referencias/pendencias-atuais.md): questões efetivamente abertas.
- [Roteiro de arquitetura](04-arquitetura/roteiro-de-decisoes.md): próxima etapa.
- [Inventário de evidências](referencias/inventario-de-evidencias.md): histórico de confirmações e mudanças.

Os documentos temáticos abaixo são cadernos de refinamento e contêm propostas e registros históricos. Em divergência com hipóteses antigas, prevalece a consolidação atual. `pendencias-do-legado.md` preserva a cronologia, não é a lista atual de questões abertas.

Documentação de produto, negócio, segurança e engenharia para uma nova aplicação .NET e React da plataforma Grupo Recovery. A inclusão dos módulos no MVP foi acordada; regras detalhadas e escolhas técnicas continuam em refinamento.

## Leitura inicial

1. [Visão e escopo](01-produto/visao-e-escopo-mvp.md).
2. [Glossário](01-produto/glossario.md).
3. [Atores e responsabilidades](01-produto/atores-e-responsabilidades.md).
4. [Registro inicial de regras](02-negocio/registro-de-regras.md).
5. [Modelo de autorização](03-seguranca/modelo-de-autorizacao.md).
6. [Pendências e decisões provisórias](referencias/pendencias-do-legado.md).
7. [Evidências](referencias/inventario-de-evidencias.md).
8. [Backlog de refinamento](06-entrega/backlog-mvp.md).
9. [Escopos de escritórios e carteiras](02-negocio/escopos-carteiras-agencias-paises.md).
10. [Identidade e ambiente de testes](04-arquitetura/identidade-e-ambiente-de-testes.md).
11. [Jornadas do devedor](02-negocio/portal-do-devedor.md).
12. [Administração do Portal e cadastros](01-produto/administracao-portal-e-cadastros.md).
13. [Administração de identidade e delegação](02-negocio/administracao-de-identidade.md).
14. [Usuários: cadastro e ciclo de vida](02-negocio/usuarios.md).
15. [Sessões e logout](02-negocio/sessoes-e-logout.md).
16. [Autenticação, senhas e MFA](02-negocio/autenticacao-e-senhas.md).
17. [Grupos e hierarquias](02-negocio/grupos-e-hierarquias.md).
18. [Restrições de IP](02-negocio/restricoes-ip-e-acesso-externo.md).
19. [Perfis e permissões](02-negocio/perfis-e-permissoes.md).
20. [Microfrontends](04-arquitetura/microfrontends.md).
21. [Auditoria e revogação](03-seguranca/auditoria-e-revogacao.md).
22. [Workflow: recorte em refinamento](02-negocio/workflow.md).
23. [Cadastros: regras transversais](02-negocio/cadastros-regras-transversais.md).

## Convenções

Cada regra deve registrar ID, descrição, evidência, estado da decisão, critérios de aceite e dependências. IDs são estáveis; regras substituídas permanecem rastreáveis.

Evidência: **relatada na transcrição**, **verificada na fonte** ou **proposta para o novo sistema**. Uma regra pode ter várias evidências. Nesta versão nenhuma implementação legada foi verificada diretamente.

Estado da decisão: **proposta**, **provisória**, **aprovada** ou **substituída**. Aprovação exige registro de responsável e data. Inclusão no escopo não aprova automaticamente o comportamento descrito no legado.

Decisão provisória deve indicar justificativa, impacto, responsável pela validação (ou a definir), condição de revisão e entregas afetadas. Ausência de resposta não significa aprovação.

## Evolução

Os módulos serão desdobrados em documentos próprios com fluxos, estados, validações e critérios de aceite. Arquitetura .NET/React, dados, contratos OpenAPI, operação e testes serão documentados conforme as regras amadurecerem. Não há escolha de versões, bibliotecas, hospedagem ou banco aprovada nesta versão.
