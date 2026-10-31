# Perfis e permissões — catálogo inicial

## Decisões E26

Confirmadas pelo solicitante em 2026-10-31:

| ID | Regra | Estado |
|---|---|---|
| ACL-003 | Separar ações de cadastro em consultar, criar, editar, inativar/reativar e exportar | Base aprovada; exceções a refinar |
| ACL-004 | Classificar gestão de usuários, acessos e configurações de segurança como administração | Base aprovada; exceções a refinar |
| IAM-019 | Perfil global recebe automaticamente novas ações comuns adicionadas à plataforma | Aprovada |

Não classificar toda ação de escrita como administrativa: operações comuns de cadastro podem incluir escrita. Classificação é uma decisão de negócio e segurança registrada por ação; o catálogo definitivo ainda precisa de revisão.

## Consequências

- A classificação determina herança, cópia e alcance do perfil global. Ação administrativa não entra implicitamente nesses mecanismos de concessão comum.
- Nova ação comum amplia automaticamente o perfil global (IAM-019); isso não dispensa contexto ativo, MFA ou restrições IP.
- Classificação inicial e mudanças de comum para administrativa ou vice-versa precisam ser rastreáveis. Responsável e processo de classificação ainda não definidos.
- Exportação pertence à base inicial de ações, mas campos, volume e eventuais exceções administrativas continuam pendentes.

## Critérios de aceite iniciais

- Permitir atribuir consulta sem conceder edição ou exportação automaticamente.
- Nova ação classificada como comum torna-se disponível ao perfil global, sujeita aos demais controles.
- Nova ação administrativa não é concedida ao perfil global apenas por sua criação.
- Permissões de portal, módulo, menu e endpoint devem ser relacionadas explicitamente; menu visível não concede autorização de backend.
