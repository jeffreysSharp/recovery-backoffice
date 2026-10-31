# Autenticação, senhas e MFA

## Regras confirmadas

| ID | Regra | Decisão e origem |
|---|---|---|
| AUTH-003 | Exigir MFA para todos os usuários do Backoffice | Aprovada, E18 |
| AUTH-004 | Após 5 tentativas consecutivas de login com senha inválida, bloquear novas autenticações por 15 minutos | Provisória, E18 |
| PWD-002 | Disponibilizar recuperação por autosserviço com link enviado ao email cadastrado e confirmado e redefinição por administrador autorizado | Aprovada em capacidade, E18; fluxos e limites pendentes |
| AUTH-005 | Somente Recovery pode autorizar recuperação de MFA, após verificar identidade | Aprovada, E19; procedimento de verificação pendente |
| PWD-003 | Redefinição administrativa usa link para o usuário definir nova senha | Aprovada, E19; validade e envio a detalhar |

E19 mantém método de MFA pendente por orientação do solicitante; não bloqueia refinamento dos demais módulos. MFA continua obrigatório (AUTH-003). Primeiro acesso preserva senha temporária (USR-006/011/012); redefinição administrativa não reutiliza esse mecanismo.

Regras já definidas em [usuários](usuarios.md): login por CPF, conta única, senha temporária gerada pelo sistema e enviada ao email individual, validade de 24 horas e troca obrigatória. [Sessões](sessoes-e-logout.md) define sessão única e tempos provisórios.

## Fluxos a detalhar

- **Primeiro acesso:** verificar senha temporária, realizar troca obrigatória e cadastrar/verificar MFA antes do acesso normal. A ordem final e a autorização restrita de cada etapa são propostas, não desenho concluído.
- **Login normal:** autenticar credencial e segundo fator; somente então concluir entrada na aplicação. Não considerar senha correta isoladamente como autenticação completa.
- **Recuperação pelo usuário:** link de uso único vinculado à conta destinatária; validade, reemissão e invalidação de sessões a decidir. Não permitir que parâmetro enviado pelo navegador altere a titularidade da solicitação.
- **Redefinição administrativa:** verificar alcance do administrador e gerar link para o usuário definir nova senha (PWD-003). Validade, destinatário e tratamento de email ainda não confirmado precisam de definição; não aplicar automaticamente as 24 horas da senha temporária ao link.
- **Perda de MFA:** processo separado de recuperação de senha, autorizado exclusivamente pela Recovery após verificar identidade (AUTH-005). Evidências aceitas, executores e reinscrição ainda pendentes. Ter acesso ao email não deve ser presumido como autorização automática para remover MFA.

As descrições de segurança acima são propostas técnicas para refinamento. Não representam escolha de provedor, biblioteca ou método de MFA.

## Bloqueio provisório

Contagem compartilhada por conta é proposta para impedir que a troca de navegador contorne o limite. Definir janela de contagem, momento de reinicialização, tratamento de contas inexistentes, abuso por bloqueio intencional e limitação de requisições por origem. Tentativas de MFA e falhas técnicas precisam de política própria; não incluí-las automaticamente nas cinco senhas inválidas.

Não foi decidido que bloqueio por senha inválida encerra uma sessão já autenticada. Distinguir esse evento de bloqueio administrativo e das revogações previstas em SES-002.

## Critérios de aceite iniciais

- Nenhum usuário do Backoffice conclui login normal somente com senha, inclusive administradores.
- Quinta senha inválida consecutiva aciona bloqueio de novas autenticações por 15 minutos; definição dos contadores ainda pendente.
- Usuário pode iniciar recuperação por autosserviço e administrador autorizado pode iniciar redefinição dentro do seu alcance.
- Solicitação de recuperação de uma conta não pode redefinir a senha de outra.
- Tratar mensagens sem expor indevidamente existência ou estado da conta; texto e resposta ainda a especificar.

## Pendências

Método e inscrição de MFA, recuperação do fator, eventual IdP, validade do link, reautenticação em ações sensíveis, política da senha definitiva e revogação após redefinição. Obrigação de MFA não significa que a oferta gratuita Azure já atenda ao desenho escolhido.
