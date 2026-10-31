# Documentação do MVP Recovery Backoffice

Início: 2026-10-31. Estado: refinamento inicial.

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

## Convenções

Cada regra deve registrar ID, descrição, evidência, estado da decisão, critérios de aceite e dependências. IDs são estáveis; regras substituídas permanecem rastreáveis.

Evidência: **relatada na transcrição**, **verificada na fonte** ou **proposta para o novo sistema**. Uma regra pode ter várias evidências. Nesta versão nenhuma implementação legada foi verificada diretamente.

Estado da decisão: **proposta**, **provisória**, **aprovada** ou **substituída**. Aprovação exige registro de responsável e data. Inclusão no escopo não aprova automaticamente o comportamento descrito no legado.

Decisão provisória deve indicar justificativa, impacto, responsável pela validação (ou a definir), condição de revisão e entregas afetadas. Ausência de resposta não significa aprovação.

## Evolução

Os módulos serão desdobrados em documentos próprios com fluxos, estados, validações e critérios de aceite. Arquitetura .NET/React, dados, contratos OpenAPI, operação e testes serão documentados conforme as regras amadurecerem. Não há escolha de versões, bibliotecas, hospedagem ou banco aprovada nesta versão.
