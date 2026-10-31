# Escopos de escritórios, carteiras e países

## Confirmações de negócio

Fonte E03, respostas do solicitante em 2026-10-31:

- SCP-002: um usuário pode atuar em mais de um escritório de cobrança. Evidência: confirmação do solicitante. Decisão: aprovada quanto à capacidade de múltiplos vínculos.
- SCP-003: um escritório pode acessar mais de uma carteira. Evidência: confirmação do solicitante. Decisão: aprovada quanto à capacidade de múltiplas carteiras.
- SCP-004: existem usuários com atuação ampla e usuários restritos a escritório(s). Evidência: confirmação do solicitante. Decisão: aprovada quanto à existência desses alcances; significado exato do alcance amplo pendente.

Essas confirmações não aprovam acesso administrativo global nem herança automática entre grupos.

Fonte E04, complemento do solicitante em 2026-10-31:

- SCP-005: uma carteira pode ser atendida por vários escritórios. Evidência: confirmação do solicitante. Decisão: aprovada.
- SCP-006: cada escritório autorizado à carteira pode visualizar todos os devedores/casos dessa carteira, sem restrição aos casos individualmente atribuídos ao escritório. Evidência: confirmação do solicitante. Decisão: aprovada para visibilidade; edição, exportação e ações financeiras dependem de permissões próprias.

## Modelo candidato

Separar vínculo de usuário com escritório, vínculo de escritório com carteira e concessão de ações. Agência e escritório são termos potencialmente relacionados, sem equivalência confirmada.

- SCP-007: todos os usuários de um escritório recebem o escopo de todas as carteiras vinculadas a esse escritório. Não há recorte individual de carteiras dentro do escritório nesta regra. Fonte E05, confirmação do solicitante em 2026-10-31; decisão aprovada. Ações disponíveis continuam limitadas pelas permissões do usuário.

O alcance amplo deve ser explícito e auditável. Não inferir esse alcance a partir de ausência de vínculo ou de filtros vazios. Diretriz proposta, sujeita à validação do modelo de autorização.

## Questões abertas

1. Quais estados dos vínculos tornam o escopo efetivo e como remover o acesso quando um vínculo é inativado?
2. Quais ações sobre os casos são permitidas a cada perfil, além da visualização aprovada?
3. Usuário vinculado a vários escritórios escolhe contexto ativo ou consulta o conjunto autorizado?
4. País e empresa são fronteiras obrigatórias? Qual é o alcance dos usuários amplos?
5. Escritório é equivalente à agência do legado?

## Cenários de aceite a completar

- Usuário restrito ao escritório A não pode acessar registros exclusivos de B, inclusive por API e exportação.
- Se A e B estão autorizados à mesma carteira, um usuário com concessão de consulta dessa carteira em A pode consultar também casos atribuídos a B nessa carteira. A atribuição operacional não restringe a visibilidade aprovada em SCP-006.
- Usuário vinculado a A e B pode operar conforme as concessões de cada vínculo; a composição ainda precisa ser decidida.
- Remoção de vínculo deve retirar o acesso correspondente dentro do prazo de revogação a definir.

Os cenários aplicam SCP-005/006/007 e propostas de proteção de escopo. Permissões por ação continuam pendentes. “Exclusivo de B” significa fora das carteiras autorizadas ao usuário em A, não apenas atribuído operacionalmente a B.
