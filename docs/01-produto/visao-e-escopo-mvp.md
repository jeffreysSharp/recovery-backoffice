# Visão e escopo do MVP

## Objetivo

Construir uma nova aplicação Backoffice da plataforma Grupo Recovery, com backend .NET e frontend React. O primeiro MVP tem foco em Identidade, Administração do Portal, microfrontends e cadastros básicos, conforme direcionamento E05. O documento legado é referência de investigação, não especificação aprovada do novo sistema.

## Escopo acordado

| Módulo | Cobertura a refinar |
|---|---|
| Administração do Portal | Administração do Backoffice e do portal do devedor confirmada; catálogo de módulos, navegação e configurações a refinar |
| Microfrontends | Composição do portal React, integração de módulos e contrato com autenticação/autorização; tecnologia específica pendente |
| Cadastros básicos | Lista inicial: cidade, estado, país, sexo, gênero, tipo de documento, marcas/veículos; detalhamento adiado pelo solicitante |
| Usuários | Cadastro, consulta, edição, ativação, desativação e responsabilidades que impedem operações |
| Autenticação | Login, logout, integração corporativa ou credenciais locais, acesso externo |
| Senhas | Troca, recuperação, expiração e administração, quando aplicáveis ao provedor escolhido |
| Sessões | Expiração, concorrência e revogação |
| Grupos e hierarquias | Tipos, vínculos, administração e alcance hierárquico |
| Perfis e permissões | Objetos, ações, atribuições e composição de permissões |
| Escopos | Carteiras, agências e países; limites por organização a confirmar |
| IP | Faixas, exceções, origem confiável do endereço e momento da validação |
| Workflow | Papéis pessoais/funcionais, responsáveis, leitura, escrita e transições |
| Inatividade | Bloqueio, desativação e exceções |
| Auditoria | Acesso, alterações, atribuições e ações sensíveis |
| Propagação | Prazo e mecanismo para efetivar mudanças de acesso |

Todos esses módulos pertencem ao MVP. Profundidade de cada entrega e ordem de implementação ainda serão definidas.

## Limites ainda não decididos

- Devedor é público previsto da plataforma. Consulta de dívidas, boletos e acompanhamento de acordos permanecem registrados como contexto futuro, fora do refinamento e da entrega inicial do Backoffice por direcionamento E05.
- Cálculos financeiros, negociação e integrações de boletos não fazem parte do foco inicial. Não inferir motor financeiro ou jornadas operacionais completas da inclusão de Workflow.
- Workflow, hierarquias, IP e escopos continuam no escopo de documentação acordado. Para Workflow, distinguir administração de papéis/configurações de execução dos processos; profundidade de implementação a confirmar no backlog.
- Migração de usuários, vínculos, senhas, histórico e coexistência com o SIR: estratégia pendente.
- Países, empresas, volume de usuários, disponibilidade e implantação: pendentes.

## Critério para iniciar uma funcionalidade

Fluxo e critérios de aceite definidos, autorização e escopo explícitos e dependências conhecidas. Premissas reversíveis podem ser provisórias e registradas. Questões que determinam exposição de dados ou identidade exigem decisão antes de liberar o fluxo dependente. As demais entregas continuam.
