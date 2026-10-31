# Grupos e hierarquias

## Regra confirmada

HIE-002 (E20): grupo superior herda permissões dos grupos inferiores. Decisão aprovada pelo solicitante em 2026-10-31. Não importar a ausência de herança do caminho legado como regra do novo sistema.

O escopo de escritório ativo (IAM-008) e o limite delegável Recovery (IAM-012) continuam aplicáveis. A herança não aprova acesso automático a outros escritórios nem transforma administrador de escritório em administrador Recovery.

Complemento E21, confirmado pelo solicitante em 2026-10-31:

- HIE-003: herança alcança todos os descendentes, não somente filhos diretos. Aprovada.
- HIE-004: permissões administrativas não são herdadas; exigem atribuição explícita. Aprovada.

Logo, HIE-002 é interpretada como herança transitiva de permissões comuns. Classificação de cada ação como comum/administrativa ainda precisa de catálogo. Grupo superior não adquire administração por conter descendente administrador.

## Detalhamento pendente

HIE-006 (E25): impedir ciclos na hierarquia. Aprovada em 2026-10-31. HIE-007 (E25): grupos personalizados de um escritório não podem possuir relações hierárquicas com grupos de outros escritórios. Aprovada. As propostas de prevenção de ciclos passam a regras confirmadas; múltiplos superiores continuam permitidos dentro desses limites.

Critérios: rejeitar vínculo que forme ciclo direto ou indireto; rejeitar relação entre grupos personalizados de escritórios diferentes; permitir múltiplos superiores válidos dentro do mesmo escritório. Grupos/modelos centrais Recovery e sua eventual distribuição precisam de desenho próprio; não inferir exceção a HIE-007.

HIE-005 (E24): um grupo pode ter vários superiores. Aprovada em 2026-10-31. O modelo não será limitado a árvore. Proposta técnica: grafo dirigido sem ciclos, com deduplicação de permissões alcançadas por múltiplos caminhos. A rejeição de ciclos ainda deve ser formalizada; múltiplos superiores não elimina os limites de contexto nem a exclusão de administração da herança.

ACL-002 (E22): provisoriamente, somar concessões comuns dos grupos/perfis no escritório ativo, sem negativas explícitas no MVP. Restrições de IP, estado de conta/vínculo, limites Recovery e atribuição explícita de administração continuam obrigatórios; não são superados pela união. Responsável pela validação a indicar; revisar com o time antes da liberação operacional e ao fechar o catálogo de ações.

Essa decisão resolve provisoriamente a composição citada abaixo. Não significa união de escritórios nem herança de administração.

- Catálogo que distingue ações comuns e administrativas.
- Validação de ciclos e fronteiras dos vínculos entre grupos; múltiplos superiores já aprovados em HIE-005.
- Composição de múltiplos grupos/perfis, negativas explícitas e precedência.
- Distinção entre herdar ações e herdar restrições de IP; a primeira decisão não resolve a segunda.
- Efeito de inativação e de alterações da hierarquia sobre sessões existentes.

## Critérios propostos

- Permissão herdada deve ter sua origem rastreável na consulta de acesso efetivo.
- Impedir ciclos de hierarquia e concessões acima do limite delegável.
- Avaliar ação herdada dentro do escritório ativo, sem ampliar o escopo de carteiras por efeito colateral.
- Em uma cadeia A > B > C, A herda permissão comum de C, dentro dos limites e do contexto aplicáveis.
- Permissão administrativa atribuída a C não é concedida a A ou B por herança.

Testes completos dependem das respostas sobre profundidade, composição e administração.
