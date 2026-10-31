# Administração de identidade e delegação

Fonte E07: respostas do solicitante em 2026-10-31. Regras confirmadas nesta rodada não aprovam os limites ainda abertos de delegação.

| ID | Regra | Estado |
|---|---|---|
| IAM-001 | Haverá administração central Recovery e administração por escritório; administradores de escritório também podem cadastrar usuários | Aprovada, esclarecida em E08 |
| IAM-002 | Administrador de escritório só pode gerenciar usuários no próprio escritório | Aprovada |
| IAM-003 | Criação e alteração de acessos não exigem aprovação por segunda pessoa | Aprovada |
| IAM-004 | Usuário pode pertencer a vários grupos/perfis de permissão | Aprovada |
| IAM-005 | Permissões de um usuário podem variar entre escritórios | Aprovada |
| IAM-006 | Administrador de escritório pode copiar permissões de outro usuário | Aprovada em capacidade; origem e semântica da cópia pendentes |
| IAM-007 | Administrador de escritório pode nomear outros administradores do próprio escritório | Aprovada em E08; mantém o limite IAM-002 |
| IAM-008 | Usuário com vínculos em vários escritórios deve escolher um escritório ativo; suas permissões operacionais são avaliadas nesse contexto | Aprovada em E08 |
| IAM-009 | Na cópia realizada pelo administrador de escritório, origem e destino devem pertencer ao próprio escritório | Aprovada em E09 |
| IAM-010 | Ao copiar, o administrador escolhe acrescentar ou substituir permissões do destino | Aprovada em E09; conteúdo exato da cópia pendente |
| IAM-011 | A Recovery define os limites de acesso delegáveis; o escritório personaliza seus grupos/perfis dentro desses limites | Aprovada em E09; representação dos limites pendente |
| IAM-012 | O limite de permissões delegáveis definido pela Recovery é único para todos os escritórios | Aprovada em E10; não implica permissões efetivas iguais |
| IAM-013 | A cópia inclui somente permissões comuns; não concede administração | Aprovada em E10 |
| IAM-014 | Origem e destino ficam independentes após a cópia; alterações futuras nas atribuições da origem não sincronizam o destino | Aprovada em E10 |

Fonte E10: respostas do solicitante em 2026-10-31. A nomeação de administrador continua disponível como operação própria (IAM-007), separada da cópia. A representação de permissões comuns em grupos/perfis ainda precisa de desenho.

Complemento E13: IAM-003 possui exceção específica: vínculo de conta existente com outro escritório exige autorização Recovery (USR-013). Administrador de escritório também pode alterar CPF/email da identidade global (USR-014), com controles ainda a definir.

Atualização E15: a autorização de CPF acima foi substituída: somente Recovery pode alterar CPF (USR-016). Usuário edita seu email e perfil pessoal; manutenção da edição de email pelo administrador do escritório precisa de confirmação. Não implementar a autorização antiga de CPF para escritório.

Fonte E09: respostas do solicitante em 2026-10-31. Os limites de IAM-011 também se aplicam à personalização e à cópia, sem ampliar o alcance administrativo do escritório.

Fonte E08: complemento do solicitante em 2026-10-31. A confirmação de cadastro por administrador de escritório esclarece a restrição inicialmente ambígua de IAM-001; não concede administração de outros escritórios.

## Consequências para o modelo

E26 confirma IAM-019: novas ações comuns entram automaticamente no perfil global. A base de classificação distingue ações de cadastro e gestão administrativa, com exceções a refinar no [catálogo de permissões](perfis-e-permissoes.md). Perguntas históricas sobre novas ações ficam resolvidas por essa decisão.

Atualização E25, confirmada em 2026-10-31:

- IAM-017: somente administradores centrais autorizados Recovery podem conceder/revogar o perfil global. Aprovada.
- IAM-018: perfil global concede seleção de qualquer escritório e todas as ações comuns. Aprovada; substitui a hipótese anterior de seleção sem concessão de ações comuns.
- Administração continua exigindo atribuição explícita. MFA, escritório ativo e restrições IP permanecem obrigatórios.
- Catálogo de ações comuns versus administrativas é necessário para aplicar essa regra. Efeito sobre futuras ações comuns e alcance do limite delegável para usuários internos ainda precisam de esclarecimento.

O estado E25 prevalece sobre perguntas e hipóteses históricas de E24 abaixo. Perfil global não deve ser concedido por cópia de permissões ou administração de escritório; cópia comum e perfil global são operações distintas.

E24, confirmado em 2026-10-31:

- IAM-016: perfil global de usuário Recovery permite selecionar qualquer escritório, sem necessidade de atribuir cada vínculo explicitamente. Aprovada.
- Perfil global amplia a elegibilidade de seleção, mantendo escritório ativo (IAM-015) e políticas IP aplicáveis. Não concede automaticamente todas as ações, dispensa MFA ou supera restrições.
- Quem pode conceder/revogar o perfil global e quais permissões operacionais acompanham esse perfil ainda precisam de definição; não incluí-lo implicitamente na administração delegada dos escritórios.

Essa decisão resolve a pergunta histórica abaixo sobre como representar elegibilidade ampla; não introduz contexto operacional sem escritório.

E23 confirma IAM-015: usuários administradores Recovery também operam com escritório ativo. A proposta de contexto administrativo próprio separado não foi adotada. Definir se usuários internos de alcance amplo recebem vínculos explícitos ou outro mecanismo de elegibilidade, sem presumir administração global sem contexto.

E23 confirma etapa pré-contexto restrita: após verificações de identidade aplicáveis, seleção sem dados operacionais somente quando o IP for elegível em ao menos um escritório autorizado. Detalhes em [IP](restricoes-ip-e-acesso-externo.md). Não encerrar sessão anterior por mero acesso à seleção; momento de conclusão do novo login ainda a especificar com SES-003.

- O acesso não pode ser representado somente por uma lista global de perfis do usuário. A autorização precisa preservar o escritório ao qual a concessão se aplica.
- O acesso às carteiras do escritório segue SCP-007; ações disponíveis são determinadas pelas concessões no respectivo contexto.
- Ausência de dupla aprovação não elimina validação de delegação nem auditoria. Proposta: registrar ator, destino, contexto e alterações de privilégios.
- Não estender uma permissão concedida no escritório A ao escritório B apenas porque a identidade participa de ambos.

Os dois últimos itens são diretrizes propostas de segurança, a detalhar em critérios de aceite e composição das permissões.

## Questões abertas

1. Conta existente exige autorização Recovery para novo escritório (E13). Definir solicitação, autoridade que decide, registro da autorização e dados mínimos expostos ao solicitante.
2. Quais grupos/perfis pode conceder? Pode alterar o próprio acesso ou conceder permissões superiores às suas? Nomear administradores do próprio escritório está aprovado, sem implicar administração central Recovery.
3. E13 autoriza administrador de escritório a alterar CPF/email globais; detalhar confirmação, notificação e efeitos em sessões/recuperação. Demais campos globais ainda a delimitar.
4. Usuário com várias concessões no mesmo contexto: união das permissões, negativas explícitas ou precedência?
5. A cópia é restrita ao próprio escritório, permite acrescentar/substituir, exclui administração e não sincroniza atribuições futuras da origem (E09/E10). Ainda definir representação em grupos/perfis ou concessões diretas e tratamento de perfis que misturem permissões comuns e administrativas.
6. Qual tratamento para troca de escritório com formulários abertos e operações em andamento? Como selecionar contexto para administradores centrais com alcance amplo?

## Critérios de aceite iniciais

- Não solicitar aprovação de segunda pessoa como etapa obrigatória de criação/alteração de acesso (IAM-003).
- Representar múltiplos grupos/perfis e concessões por escritório (IAM-004/005).
- Negar tentativa de administrar vínculo de escritório diferente do autorizado (IAM-002).
- Detalhar testes de identidade global, autoelevação e concessão de perfis após resolver as questões acima.
- Permitir cadastro por administrador do próprio escritório e nomeação de outro administrador nesse limite (IAM-001/007).
- Aplicar permissões do escritório ativo; concessão em A não autoriza ação em B (IAM-005/008).
- Proposta: revalidar vínculo e permissão no servidor em cada requisição, sem confiar apenas no escritório selecionado na interface.

## Cópia de permissões — fluxo inicial

1. Administrador seleciona origem e destino no próprio escritório (IAM-009).
2. Escolhe acrescentar ou substituir (IAM-010).
3. Proposta: apresentar diferenças antes de executar, incluindo concessões comuns removidas ou adicionadas; excluir concessões administrativas da cópia.
4. Validar no servidor o alcance do administrador, os vínculos e os limites definidos pela Recovery (IAM-002/011).
5. Proposta: aplicar a alteração de forma atômica no contexto escolhido e registrar ator, origem, destino, modo e diferenças.

Semântica proposta: acrescentar preserva concessões existentes e adiciona as selecionadas sem duplicação; substituir troca somente o conjunto copiado no escritório ativo. Não remover identidade, vínculo de escritório ou acesso em outros contextos por efeito colateral. O conjunto copiado ainda precisa de decisão antes de implementar.

E10 limita o conjunto a permissões comuns. Proposta: o modo substituir também preserva a administração preexistente do destino; revogar ou conceder administração exige operação própria. Independência da origem não implica congelamento das definições de perfis compartilhados ou do limite global: o efeito dessas mudanças continua pendente.

Não foi aprovado incluir carteiras, dados pessoais, responsabilidades operacionais de Workflow ou condições de autenticação na cópia de permissões.

## Critérios adicionais

- Rejeitar cópia com origem ou destino fora do escritório administrado, inclusive por requisição direta à API.
- Exigir modo explícito de cópia; não presumir substituição ou acréscimo.
- Rejeitar concessões fora dos limites da Recovery. Tratamento de permissões antigas acima do limite e alterações simultâneas permanece a definir.
- Não disponibilizar criação ou edição de grupos/perfis além do limite delegável aprovado para o escritório.
- Alterações futuras nas atribuições da origem não devem alterar as atribuições do destino por efeito da cópia (IAM-014).
- A cópia não pode conceder administração, mesmo quando a origem for administradora (IAM-013).
