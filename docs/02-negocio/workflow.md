# Workflow — recorte em refinamento

Workflow permanece no escopo de documentação do MVP. E34 (2026-10-31) solicita definir o recorte com mais detalhes; não foi aprovado limitar a entrega a cadastros administrativos nem incluir automaticamente execução completa de processos.

## Direcionamento E35 — 2026-10-31

WFL-003: construir a execução de Workflow na nova aplicação, em vez de assumir operação do processo por integração com o legado. Decisão aprovada pelo solicitante. Isso não elimina eventuais integrações de dados nem define um motor genérico de processos.

Solicitante adiou o exemplo do processo e o detalhamento funcional. Perguntas do roteiro abaixo ficam pendentes, sem bloquear refinamento dos outros módulos. Estados, transições, atores, alçadas e critérios de execução ainda não podem ser considerados especificados para implementação.

## Referência disponível

E01 relata papéis pessoais/funcionais, vínculos de usuários, responsáveis, permissões de leitura/escrita, sistemas, pastas, mensagens e transições. São conceitos do legado; não são nomes ou estruturas obrigatórios para o novo sistema. Cálculos financeiros e jornadas operacionais completas não estão especificados pela transcrição.

## Decisão confirmada relacionada

USR-022 (E34): retirar acesso ao desativar usuário mesmo com responsabilidades pendentes; transferência ocorre posteriormente. Preservar rastreabilidade da responsabilidade; não convertê-la em concessão de acesso ao usuário desativado.

## Roteiro para fechar o recorte

1. Qual problema ou processo concreto precisa ser atendido na primeira entrega?
2. Quais objetos são administrados: papéis, equipes, etapas, filas, tarefas ou outros?
3. Quais ações serão executadas pelo usuário e quais apenas configuradas pelo administrador?
4. Onde ficam os processos e responsabilidades existentes: novo sistema ou integração com legado?
5. Quais dados/estados, transições, alçadas e critérios de aceite definem a menor entrega utilizável?

Não escolher motor de Workflow ou reproduzir tabelas do legado antes de esclarecer esse recorte.
